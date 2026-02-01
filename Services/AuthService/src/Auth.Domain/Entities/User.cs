using System;

namespace Auth.Domain.Entities;

public sealed class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;
    public string? Phone { get; set; }

    public string PasswordHash { get; set; } = null!;

    public UserStatus Status { get; set; } = UserStatus.Active;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }
    public int FailedLoginCount { get; set; }
}