using Auth.Application.Abstractions.Persistence;
using Auth.Infrastructure.Persistence;

namespace Auth.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AuthDbContext _db;
    public UnitOfWork(AuthDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}