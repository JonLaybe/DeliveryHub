using Auth.Application.Abstractions.Persistence;
using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Persistence.Repositories;

public sealed class EfUserRepository : IUserRepository
{
    private readonly AuthDbContext _db;
    public EfUserRepository(AuthDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Users.SingleOrDefaultAsync(x => x.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        _db.Users.SingleOrDefaultAsync(x => x.Email == email, ct);

    public void Add(User user) => _db.Users.Add(user);
}