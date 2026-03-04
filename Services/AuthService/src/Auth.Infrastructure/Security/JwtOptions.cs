namespace Auth.Infrastructure.Security;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = "AuthService";
    public int AccessTokenMinutes { get; init; } = 15;
    public int RefreshTokenDays { get; init; } = 30;
    public string KeyId { get; init; } = "auth-key-001";
    public string PrivateKeyPem { get; init; } = null!;
    public string PublicKeyPem { get; init; } = null!;
}