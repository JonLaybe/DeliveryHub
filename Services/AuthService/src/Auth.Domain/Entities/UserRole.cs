using System;
using Microsoft.AspNetCore.Identity;

namespace Auth.Domain.Entities;

public sealed class UserRole : IdentityUserRole<Guid>
{
    public DateTimeOffset AssignedAt { get; set; }

    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}