using Auth.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.Application.Abstractions.Persistence;

public interface IUserRoleRepository
{
    Task<bool> ExistsAsync(Guid userId, Guid roleId, CancellationToken ct = default);
    void Add(UserRole userRole);
}