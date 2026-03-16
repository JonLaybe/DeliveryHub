using System;
using Microsoft.AspNetCore.Identity;

namespace Auth.Domain.Entities;

public sealed class Role : IdentityRole<Guid>
{
    public string? Description { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}