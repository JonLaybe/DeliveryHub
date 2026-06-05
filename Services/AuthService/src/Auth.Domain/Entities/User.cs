using System;
using Microsoft.AspNetCore.Identity;

namespace Auth.Domain.Entities;

public sealed class User : IdentityUser<Guid>
{
    public UserStatus Status { get; set; } = UserStatus.Active;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhotoUrl { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}