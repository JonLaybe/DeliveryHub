using Auth.Application.Abstractions.Persistence;
using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Persistence.Repositories;

public sealed class EfUserRoleRepository : IUserRoleRepository
{
    private readonly AuthDbContext _db;
    public EfUserRoleRepository(AuthDbContext db) => _db = db;

    public Task<bool> ExistsAsync(Guid userId, Guid roleId, CancellationToken ct = default) =>
        _db.UserRoles.AnyAsync(x => x.UserId == userId && x.RoleId == roleId, ct);

    public void Add(UserRole userRole) => _db.UserRoles.Add(userRole);
}