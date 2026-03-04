using System;
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
        var privatePem =
            !string.IsNullOrWhiteSpace(opt.PrivateKeyPem) ? opt.PrivateKeyPem :
            !string.IsNullOrWhiteSpace(opt.PrivateKeyPath) ? File.ReadAllText(opt.PrivateKeyPath) :
            null;

        var publicPem =
            !string.IsNullOrWhiteSpace(opt.PublicKeyPem) ? opt.PublicKeyPem :
            !string.IsNullOrWhiteSpace(opt.PublicKeyPath) ? File.ReadAllText(opt.PublicKeyPath) :
            null;

        if (string.IsNullOrWhiteSpace(privatePem) || string.IsNullOrWhiteSpace(publicPem))
            throw new InvalidOperationException("JWT RSA keys are not configured (provide PEM or Path).");

        Private = RSA.Create();
        Private.ImportFromPem(privatePem);

        Public = RSA.Create();
        Public.ImportFromPem(publicPem);
    }
}