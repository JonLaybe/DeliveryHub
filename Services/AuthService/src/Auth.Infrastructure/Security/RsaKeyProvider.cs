using System.Security.Cryptography;

namespace Auth.Infrastructure.Security;

public interface IRsaKeyProvider
{
    RSA Private { get; }
    RSA Public { get; }
}

public sealed class RsaKeyProvider : IRsaKeyProvider
{
    public RSA Private { get; }
    public RSA Public { get; }

    public RsaKeyProvider(JwtOptions opt)
    {
        Private = RSA.Create();
        Private.ImportFromPem(opt.PrivateKeyPem);

        Public = RSA.Create();
        Public.ImportFromPem(opt.PublicKeyPem);
    }
}