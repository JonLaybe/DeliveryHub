using Auth.Domain.Entities;
using Auth.Infrastructure.Persistence;
using Auth.Infrastructure.Persistence.Entities;
using Auth.Infrastructure.Security;
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

    public record LoginRequest(string Email, string Password);
    public record RefreshRequest(string RefreshToken);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
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