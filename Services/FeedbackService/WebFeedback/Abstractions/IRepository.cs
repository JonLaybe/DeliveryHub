namespace WebFeedback.Abstractions
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetAsync(int id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);

        Task DeleteAsync(int id);
    }
}
