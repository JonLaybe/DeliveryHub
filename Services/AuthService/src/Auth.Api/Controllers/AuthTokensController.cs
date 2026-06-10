using Auth.Domain.Entities;
using Auth.Infrastructure.Persistence;
using Auth.Infrastructure.Persistence.Entities;
using Auth.Infrastructure.Security;
using Auth.Application.UseCases.Users;
using Microsoft.AspNetCore.Identity;
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
public sealed class AuthTokensController : ControllerBase
{
    private readonly UserManager<User> _users;
    private readonly SignInManager<User> _signIn;
    private readonly AuthDbContext _db;
    private readonly JwtOptions _jwt;
    private readonly IJwtTokenService _tokens;
    private readonly CreateUser _createUser;
    private readonly ILogger<AuthTokensController> _logger;

    public AuthTokensController(
        UserManager<User> users,
        SignInManager<User> signIn,
        AuthDbContext db,
        JwtOptions jwt,
        IJwtTokenService tokens,
        CreateUser createUser,
        ILogger<AuthTokensController> logger)
    {
        _users = users;
        _signIn = signIn;
        _db = db;
        _jwt = jwt;
        _tokens = tokens;
        _createUser = createUser;
        _logger = logger;
    }

    public record RegisterRequest(string Email, string Password);
    public record LoginRequest(string Email, string Password);
    public record LogoutRequest(string RefreshToken);
    public record RefreshRequest(string RefreshToken);

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Register attempt for user with email {Email}", req.Email);

        try
        {
            var user = await _createUser.ExecuteAsync(req.Email, req.Password, ct);

            _logger.LogInformation(
                "User registered successfully. UserId: {UserId}, Email: {Email}",
                user.Id,
                user.Email);

            return await IssueTokensAsync(user, ct); // выдаём токены сразу после регистрации
        }
        catch (InvalidOperationException ex) when (ex.Message == "EMAIL_ALREADY_EXISTS")
        {
            _logger.LogWarning(
                "Register failed. Email already exists: {Email}",
                req.Email);

            return Conflict(new { error = "EMAIL_ALREADY_EXISTS" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                ex,
                "Register failed for email {Email}. Validation error: {Error}",
                req.Email,
                ex.Message);

            return BadRequest(new { error = ex.Message }); // ошибки валидации пароля Identity
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error during registration for email {Email}",
                req.Email);

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Login attempt for user with email {Email}", req.Email);

        try
        {
            var user = await _users.FindByEmailAsync(req.Email);

            if (user is null)
            {
                _logger.LogWarning(
                    "Login failed. User with email {Email} was not found",
                    req.Email);

                return Unauthorized();
            }

            if (user.Status != UserStatus.Active)
            {
                _logger.LogWarning(
                    "Login failed. User {UserId} with email {Email} is not active. Status: {Status}",
                    user.Id,
                    user.Email,
                    user.Status);

                return Unauthorized();
            }

            var ok = await _signIn.CheckPasswordSignInAsync(
                user,
                req.Password,
                lockoutOnFailure: true);

            if (!ok.Succeeded)
            {
                _logger.LogWarning(
                    "Login failed. Invalid password for user {UserId} with email {Email}",
                    user.Id,
                    user.Email);

                return Unauthorized();
            }

            _logger.LogInformation(
                "User logged in successfully. UserId: {UserId}, Email: {Email}",
                user.Id,
                user.Email);

            return await IssueTokensAsync(user, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error during login for email {Email}",
                req.Email);

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Refresh token request received");

        try
        {
            var now = DateTimeOffset.UtcNow;
            var incomingHash = Sha256Hex(req.RefreshToken);

            var stored = await _db.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == incomingHash, ct);

            if (stored is null)
            {
                _logger.LogWarning("Refresh failed. Refresh token was not found");

                return Unauthorized();
            }

            if (stored.RevokedAt is not null)
            {
                _logger.LogWarning(
                    "Refresh failed. Refresh token was revoked. UserId: {UserId}",
                    stored.UserId);

                return Unauthorized();
            }

            if (stored.ExpiresAt <= now)
            {
                _logger.LogWarning(
                    "Refresh failed. Refresh token expired. UserId: {UserId}, ExpiresAt: {ExpiresAt}",
                    stored.UserId,
                    stored.ExpiresAt);

                return Unauthorized();
            }

            var user = await _users.FindByIdAsync(stored.UserId.ToString());

            if (user is null)
            {
                _logger.LogWarning(
                    "Refresh failed. User {UserId} was not found",
                    stored.UserId);

                return Unauthorized();
            }

            if (user.Status != UserStatus.Active)
            {
                _logger.LogWarning(
                    "Refresh failed. User {UserId} is not active. Status: {Status}",
                    user.Id,
                    user.Status);

                return Unauthorized();
            }

            // rotation
            var newRefresh = GenerateRefreshToken();
            var newHash = Sha256Hex(newRefresh);

            stored.RevokedAt = now;
            stored.ReplacedByHash = newHash;

            _db.RefreshTokens.Add(new RefreshTokenEntity
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = newHash,
                CreatedAt = now,
                ExpiresAt = now.AddDays(_jwt.RefreshTokenDays)
            });

            await _db.SaveChangesAsync(ct);

            var roles = await _users.GetRolesAsync(user);
            var claims = BuildUserClaims(user, roles);
            var access = _tokens.CreateAccessToken(claims, now);

            _logger.LogInformation(
                "Tokens refreshed successfully for user {UserId}",
                user.Id);

            return Ok(new
            {
                access_token = access,
                token_type = "Bearer",
                expires_in = _jwt.AccessTokenMinutes * 60,
                refresh_token = newRefresh
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during refresh token request");

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    // logout для JWT = отозвать refresh-token (access сам протухнет по времени)
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Logout request received");

        try
        {
            var now = DateTimeOffset.UtcNow;
            var hash = Sha256Hex(req.RefreshToken);

            var stored = await _db.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == hash, ct);

            if (stored is null)
            {
                _logger.LogInformation(
                    "Logout completed. Refresh token was not found, nothing to revoke");

                return NoContent();
            }

            stored.RevokedAt = now;

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Logout completed. Refresh token revoked for user {UserId}",
                stored.UserId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during logout request");

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    private async Task<IActionResult> IssueTokensAsync(User user, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        var roles = await _users.GetRolesAsync(user);
        var claims = BuildUserClaims(user, roles);

        var access = _tokens.CreateAccessToken(claims, now);

        var refresh = GenerateRefreshToken();
        var refreshHash = Sha256Hex(refresh);

        _db.RefreshTokens.Add(new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshHash,
            CreatedAt = now,
            ExpiresAt = now.AddDays(_jwt.RefreshTokenDays)
        });

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Tokens issued successfully for user {UserId}. Access token lifetime: {AccessTokenMinutes} minutes, refresh token lifetime: {RefreshTokenDays} days",
            user.Id,
            _jwt.AccessTokenMinutes,
            _jwt.RefreshTokenDays);

        return Ok(new
        {
            access_token = access,
            token_type = "Bearer",
            expires_in = _jwt.AccessTokenMinutes * 60,
            refresh_token = refresh
        });
    }

    private static List<Claim> BuildUserClaims(User user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? ""),
            new("uid", user.Id.ToString())
        };

        foreach (var r in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, r));
        }

        return claims;
    }

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static string Sha256Hex(string s)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}