using Auth.Application.Abstractions.Security;
using BCrypt.Net;

namespace Auth.Infrastructure.Security;

public sealed class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.HashPassword(password);
    public bool Verify(string password, string passwordHash) => BCrypt.Verify(password, passwordHash);
}