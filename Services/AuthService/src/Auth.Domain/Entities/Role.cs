using System;

namespace Auth.Domain.Entities;

public sealed class Role
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}