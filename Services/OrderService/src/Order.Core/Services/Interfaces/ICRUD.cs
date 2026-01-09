namespace OrderService.Core.Services.Interfaces
{
    public interface ICRUD<T, TCreate, TUpdate>
    {
        Task<T> GetEntityAsync(int id, CancellationToken cancellationToken = default);

        Task<T> AddAsync(TCreate entity, CancellationToken cancellationToken = default);

        void UpdateAsync(TUpdate entity, CancellationToken cancellationToken = default);

        Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
