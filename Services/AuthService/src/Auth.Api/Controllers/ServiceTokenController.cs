using Auth.Infrastructure.Persistence;
using Auth.Infrastructure.Security;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class ServiceTokenController : ControllerBase
{
    private readonly AuthDbContext _db;
    private readonly IJwtTokenService _tokens;
    private readonly JwtOptions _jwt;
    private readonly ILogger<ServiceTokenController> _logger;

    public ServiceTokenController(
        AuthDbContext db,
        IJwtTokenService tokens,
        JwtOptions jwt,
        ILogger<ServiceTokenController> logger)
    {
        _db = db;
        _tokens = tokens;
        _jwt = jwt;
        _logger = logger;
    }

    public record ServiceTokenRequest(string ClientId, string ClientSecret, string? Scope);

    [HttpPost("service-token")]
    public async Task<IActionResult> Issue(
        [FromBody] ServiceTokenRequest req,
        [FromServices] IValidator<ServiceTokenRequest> validator,
        CancellationToken ct)
    {
        if (req is null)
        {
            _logger.LogWarning("Service token request failed. Request body is empty");

            return BadRequest("Request body is required.");
        }

        _logger.LogInformation(
            "Service token request received for client {ClientId}",
            req.ClientId);

        try
        {
            var validationResult = await validator.ValidateAsync(req, ct);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "Service token request validation failed for client {ClientId}. Errors count: {ErrorsCount}",
                    req.ClientId,
                    validationResult.Errors.Count);

                return BadRequest(validationResult.Errors.Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                }));
            }

            var client = await _db.ServiceClients.FirstOrDefaultAsync(
                x => x.ClientId == req.ClientId,
                ct);

            if (client is null)
            {
                _logger.LogWarning(
                    "Service token request failed. Client {ClientId} was not found",
                    req.ClientId);

                return Unauthorized();
            }

            if (!client.IsActive)
            {
                _logger.LogWarning(
                    "Service token request failed. Client {ClientId} is not active",
                    req.ClientId);

                return Unauthorized();
            }

            var incomingBytes = SHA256.HashData(Encoding.UTF8.GetBytes(req.ClientSecret));
            var storedBytes = Convert.FromHexString(client.SecretHash);

            if (!CryptographicOperations.FixedTimeEquals(incomingBytes, storedBytes))
            {
                _logger.LogWarning(
                    "Service token request failed. Invalid client secret for client {ClientId}",
                    req.ClientId);

                return Unauthorized();
            }

            var now = DateTimeOffset.UtcNow;

            var requestedScopes = ParseScopes(req.Scope);

            if (requestedScopes.Count == 0)
                requestedScopes.Add("service");

            var allowedScopes = ParseScopes(client.AllowedScopes);

            var forbiddenScopes = requestedScopes
                .Where(scope => !allowedScopes.Contains(scope))
                .ToList();

            if (forbiddenScopes.Count > 0)
            {
                _logger.LogWarning(
                    "Service token request forbidden for client {ClientId}. Requested forbidden scopes: {ForbiddenScopes}",
                    req.ClientId,
                    string.Join(' ', forbiddenScopes));

                return StatusCode(StatusCodes.Status403Forbidden);
            }

            var scope = string.Join(' ', requestedScopes);

            var claims = new List<Claim>
            {
                new("sub", client.ClientId),
                new("typ", "service"),
                new("scope", scope)
            };

            var access = _tokens.CreateAccessToken(claims, now);

            _logger.LogInformation(
                "Service token successfully issued for client {ClientId}. Scope: {Scope}",
                req.ClientId,
                scope);

            return Ok(new
            {
                access_token = access,
                token_type = "Bearer",
                expires_in = _jwt.AccessTokenMinutes * 60
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while issuing service token for client {ClientId}",
                req.ClientId);

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    private static HashSet<string> ParseScopes(string? scopes)
    {
        if (string.IsNullOrWhiteSpace(scopes))
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        return scopes
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static string Sha256Hex(string value) //to del
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    private static bool FixedTimeEquals(string a, string b) //to del
    {
        var ba = Encoding.UTF8.GetBytes(a);
        var bb = Encoding.UTF8.GetBytes(b);
        return ba.Length == bb.Length && CryptographicOperations.FixedTimeEquals(ba, bb);
    }
}