using DeliveryHub.Domain.Entities;

namespace DeliveryHub.Abstractions.DataAccess
{
    public interface IRepository<T, K>
        where T : BaseEntity<K> where K : struct, IEquatable<K>
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

        IQueryable<T> GetAll();

        Task<T> GetByIdAsync(K id, CancellationToken cancellationToken);

        Task DeleteAsync(K id, CancellationToken cancellationToken);

        Task<K> CreateAsync(T entity, CancellationToken cancellationToken);

        Task UpdateAsync(T entity, CancellationToken cancellationToken);
    }
}
