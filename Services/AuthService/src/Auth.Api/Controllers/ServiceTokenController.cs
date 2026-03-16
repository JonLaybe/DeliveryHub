using Auth.Infrastructure.Persistence;
using Auth.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class ServiceTokenController : ControllerBase
{
    private readonly AuthDbContext _db;
    private readonly IJwtTokenService _tokens;
    private readonly JwtOptions _jwt;

    public ServiceTokenController(AuthDbContext db, IJwtTokenService tokens, JwtOptions jwt)
    {
        _db = db;
        _tokens = tokens;
        _jwt = jwt;
    }

    public record ServiceTokenRequest(string ClientId, string ClientSecret, string? Scope);

    [HttpPost("service-token")]
    public async Task<IActionResult> Issue([FromBody] ServiceTokenRequest req)
    {
        var client = await _db.ServiceClients.FirstOrDefaultAsync(x => x.ClientId == req.ClientId);
        if (client is null || !client.IsActive) return Unauthorized();

        var incomingBytes = SHA256.HashData(Encoding.UTF8.GetBytes(req.ClientSecret));
        var storedBytes = Convert.FromHexString(client.SecretHash); // теперь не важно upper, или lower

        if (!CryptographicOperations.FixedTimeEquals(incomingBytes, storedBytes))
            return Unauthorized();

        var now = DateTimeOffset.UtcNow;
        var scope = string.IsNullOrWhiteSpace(req.Scope) ? "service" : req.Scope.Trim();

        var claims = new List<Claim>
        {
            new("sub", req.ClientId),
            new("typ", "service"),
            new("scope", scope)
        };

        var access = _tokens.CreateAccessToken(claims, now);

        return Ok(new
        {
            access_token = access,
            token_type = "Bearer",
            expires_in = _jwt.AccessTokenMinutes * 60
        });
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