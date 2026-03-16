using Auth.Application.Abstractions.Security;
using Microsoft.AspNetCore.Identity;

namespace Auth.Api.Infrastructure.Security;

public sealed class AspNetIdentityPasswordHasher : IPasswordHasher
{
    private static readonly PasswordHasher<object> Hasher = new();

    public string Hash(string password)
        => Hasher.HashPassword(null!, password);

    public bool Verify(string password, string passwordHash)
        => Hasher.VerifyHashedPassword(null!, passwordHash, password)
           == PasswordVerificationResult.Success;
}