using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Auth.Infrastructure.Security;

public interface IJwtTokenService
{
    string CreateAccessToken(IEnumerable<Claim> claims, DateTimeOffset nowUtc);
}

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _opt;
    private readonly IRsaKeyProvider _keys;

    public JwtTokenService(JwtOptions opt, IRsaKeyProvider keys)
    {
        _opt = opt;
        _keys = keys;
    }

    public string CreateAccessToken(IEnumerable<Claim> claims, DateTimeOffset nowUtc)
    {
        var key = new RsaSecurityKey(_keys.Private) { KeyId = _opt.KeyId };
        var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

        var jwt = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: null,
            claims: claims,
            notBefore: nowUtc.UtcDateTime,
            expires: nowUtc.AddMinutes(_opt.AccessTokenMinutes).UtcDateTime,
            signingCredentials: creds);

        jwt.Header["kid"] = _opt.KeyId;
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}