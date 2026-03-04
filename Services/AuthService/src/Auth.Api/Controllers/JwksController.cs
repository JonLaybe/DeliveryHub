using Auth.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Api.Controllers;

[ApiController]
public sealed class JwksController : ControllerBase
{
    private readonly JwtOptions _opt;
    private readonly IRsaKeyProvider _keys;

    public JwksController(JwtOptions opt, IRsaKeyProvider keys)
    {
        _opt = opt;
        _keys = keys;
    }

    [HttpGet("/.well-known/jwks.json")]
    public IActionResult Get()
    {
        var key = new RsaSecurityKey(_keys.Public) { KeyId = _opt.KeyId };
        var jwk = JsonWebKeyConverter.ConvertFromSecurityKey(key);

        return Ok(new
        {
            keys = new object[]
            {
                new {
                    kty = jwk.Kty,
                    use = "sig",
                    alg = "RS256",
                    kid = _opt.KeyId,
                    n = jwk.N,
                    e = jwk.E
                }
            }
        });
    }
}