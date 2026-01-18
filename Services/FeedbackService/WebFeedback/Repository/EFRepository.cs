using Microsoft.EntityFrameworkCore;
using WebFeedback.Abstractions;
using WebFeedback.Domain;

namespace WebFeedback.Repository
{
    public class EFRepository<T> : IRepository<T> where T : Abstractions.BaseEntity
    {
        private readonly DataContext _dataContext;

        public EFRepository(DataContext context)
        {
            _dataContext = context;
        }


        public async Task<T> AddAsync(T entity)
        {
            
            await _dataContext.Set<T>().AddAsync(entity);
            await _dataContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var record = _dataContext.Set<T>().FirstOrDefault<T>(x => x.Id == id);
            if (record != null)
            {
                _dataContext.Set<T>().Remove(record);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dataContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetAsync(int id)
        {
            return await _dataContext.Set<T>().FirstOrDefaultAsync<T>(x => x.Id == id);
        }

        public async Task UpdateAsync(T entity)
        {
            _dataContext.Set<T>().Update(entity);
            await _dataContext.SaveChangesAsync();
        }
    }
}
