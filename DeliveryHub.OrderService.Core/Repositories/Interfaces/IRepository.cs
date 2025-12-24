namespace DeliveryHub.OrderService.Core.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        void UpdateAsync(T entity, CancellationToken cancellationToken = default);

        Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task<int> CreateAsync(T entity, CancellationToken cancellationToken = default);
    }
}
