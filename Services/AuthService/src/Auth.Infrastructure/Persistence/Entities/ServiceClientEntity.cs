using System;

namespace Auth.Infrastructure.Persistence.Entities;

public sealed class ServiceClientEntity
{
    public Guid Id { get; set; }
    public string ClientId { get; set; } = null!;
    public string SecretHash { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; }
    public string AllowedScopes { get; set; } = null!;//scopes одной строкой через пробел
}