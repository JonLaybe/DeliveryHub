using Auth.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    void Add(User user);
}