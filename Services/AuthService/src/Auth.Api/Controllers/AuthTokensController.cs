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

    public AuthTokensController(UserManager<User> users, SignInManager<User> signIn, AuthDbContext db, JwtOptions jwt, IJwtTokenService tokens)
    {
        _users = users;
        _signIn = signIn;
        _db = db;
        _jwt = jwt;
        _tokens = tokens;
    }

    public record RegisterRequest(string Email, string Password);
    public record LoginRequest(string Email, string Password);
    public record LogoutRequest(string RefreshToken);
    public record RefreshRequest(string RefreshToken);

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromServices] CreateUser createUser,
        [FromBody] RegisterRequest req,
        CancellationToken ct)
    {
        User user;
        try
        {
            user = await createUser.ExecuteAsync(req.Email, req.Password, ct);
        }
        catch (InvalidOperationException ex) when (ex.Message == "EMAIL_ALREADY_EXISTS")
        {
            return Conflict(new { error = "EMAIL_ALREADY_EXISTS" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

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


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var email = req.Email.Trim().ToLowerInvariant();
        var user = await _users.FindByEmailAsync(email);
        var user = await _users.FindByEmailAsync(req.Email);
        if (user is null || user.Status != UserStatus.Active) return Unauthorized();

        var ok = await _signIn.CheckPasswordSignInAsync(user, req.Password, lockoutOnFailure: true);
        if (!ok.Succeeded) return Unauthorized();

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

        await _db.SaveChangesAsync();

        return Ok(new { access_token = access, token_type = "Bearer", expires_in = _jwt.AccessTokenMinutes * 60, refresh_token = refresh });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.RefreshToken))
            return NoContent();

        var now = DateTimeOffset.UtcNow;
        var hash = Sha256Hex(req.RefreshToken);

        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hash, ct);
        if (stored is null)
            return NoContent();

        stored.RevokedAt = now; // отзываем refresh-токен
        await _db.SaveChangesAsync(ct);

        return NoContent();
    }

    // отозвать все refresh токены пользователя (по одному refresh)
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll([FromBody] LogoutRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.RefreshToken))
            return NoContent();

        var now = DateTimeOffset.UtcNow;
        var hash = Sha256Hex(req.RefreshToken);

        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hash, ct);
        if (stored is null)
            return NoContent();

        var userId = stored.UserId;

        var tokens = await _db.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiresAt > now)
            .ToListAsync(ct);

        foreach (var t in tokens)
            t.RevokedAt = now;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
    {
        var now = DateTimeOffset.UtcNow;
        var incomingHash = Sha256Hex(req.RefreshToken);

        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == incomingHash);
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

        var roles = await _users.GetRolesAsync(user);
        var claims = BuildUserClaims(user, roles);
        var access = _tokens.CreateAccessToken(claims, now);

        await _db.SaveChangesAsync();

        return Ok(new { access_token = access, token_type = "Bearer", expires_in = _jwt.AccessTokenMinutes * 60, refresh_token = newRefresh });
    }

    private static List<Claim> BuildUserClaims(User user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new("sub", user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? ""),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id.ToString()),
            new("typ", "user"),
            new("scope", "api")
        };
        foreach (var r in roles) claims.Add(new(ClaimTypes.Role, r));
        return claims;
    }

    private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    private static string Sha256Hex(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}