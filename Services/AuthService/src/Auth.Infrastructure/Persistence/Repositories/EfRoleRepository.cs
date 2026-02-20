using Auth.Application.Abstractions.Persistence;
using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Persistence.Repositories;

public sealed class EfRoleRepository : IRoleRepository
{
    private readonly AuthDbContext _db;
    public EfRoleRepository(AuthDbContext db) => _db = db;

    public Task<Role?> GetByNameAsync(string name, CancellationToken ct = default) =>
        _db.Roles.SingleOrDefaultAsync(x => x.Name == name, ct);
}