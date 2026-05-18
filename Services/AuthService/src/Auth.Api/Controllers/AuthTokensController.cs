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

    public AuthTokensController(UserManager<User> users, SignInManager<User> signIn, AuthDbContext db, JwtOptions jwt, IJwtTokenService tokens, CreateUser createUser)
    {
        _users = users;
        _signIn = signIn;
        _db = db;
        _jwt = jwt;
        _tokens = tokens;
        _createUser = createUser;
    }

    public record RegisterRequest(string Email, string Password);
    public record LoginRequest(string Email, string Password);
    public record LogoutRequest(string RefreshToken);
    public record RefreshRequest(string RefreshToken);

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken ct)
    {
        try
        {
            var user = await _createUser.ExecuteAsync(req.Email, req.Password, ct);

            return await IssueTokensAsync(user, ct); //выдаём токены сразу после регистрации
        }
        catch (InvalidOperationException ex) when (ex.Message == "EMAIL_ALREADY_EXISTS")
        {
            return Conflict(new { error = "EMAIL_ALREADY_EXISTS" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message }); //ошибки валидации пароля Identity
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var user = await _users.FindByEmailAsync(req.Email);
        if (user is null || user.Status != UserStatus.Active) return Unauthorized();

        var ok = await _signIn.CheckPasswordSignInAsync(user, req.Password, lockoutOnFailure: true);
        if (!ok.Succeeded) return Unauthorized();

        return await IssueTokensAsync(user, ct);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var incomingHash = Sha256Hex(req.RefreshToken);

        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == incomingHash, ct);
        if (stored is null || stored.RevokedAt is not null || stored.ExpiresAt <= now) return Unauthorized();

        var user = await _users.FindByIdAsync(stored.UserId.ToString());
        if (user is null || user.Status != UserStatus.Active) return Unauthorized();

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

        return Ok(new
        {
            access_token = access,
            token_type = "Bearer",
            expires_in = _jwt.AccessTokenMinutes * 60,
            refresh_token = newRefresh
        });
    }

    // logout для JWT = отозвать refresh-token (access сам протухнет по времени)
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest req, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var hash = Sha256Hex(req.RefreshToken);

        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hash, ct);
        if (stored is null) return NoContent();

        stored.RevokedAt = now;
        await _db.SaveChangesAsync(ct);

        return NoContent();
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

        foreach (var r in roles) claims.Add(new Claim(ClaimTypes.Role, r));
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