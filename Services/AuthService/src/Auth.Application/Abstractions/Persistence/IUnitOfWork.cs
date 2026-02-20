using System.Threading;
using System.Threading.Tasks;

namespace Auth.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}