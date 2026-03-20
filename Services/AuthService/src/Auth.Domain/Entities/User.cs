using System;
using Microsoft.AspNetCore.Identity;

namespace Auth.Domain.Entities;

public sealed class User : IdentityUser<Guid>
{
    public UserStatus Status { get; set; } = UserStatus.Active;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}