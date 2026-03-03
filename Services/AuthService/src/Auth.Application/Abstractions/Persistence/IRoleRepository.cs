using Auth.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.Application.Abstractions.Persistence;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);
}