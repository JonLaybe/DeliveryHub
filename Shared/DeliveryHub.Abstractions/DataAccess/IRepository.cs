using DeliveryHub.Domain.Entities;

namespace DeliveryHub.Abstractions.DataAccess
{
    public interface IRepository<T, K>
        where T : BaseEntity<K> where K : struct, IEquatable<K>
    {
        Task<IEnumerable<T>> GetAllAsync();

        IQueryable<T> GetAll();

        Task<T> GetByIdAsync(K id);

        Task DeleteAsync(K id);

        Task<K> CreateAsync(T entity);

        Task UpdateAsync(T entity);
    }
}
