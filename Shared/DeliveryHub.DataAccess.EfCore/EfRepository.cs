using DeliveryHub.Abstractions.DataAccess;
using DeliveryHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeliveryHub.DataAccess.EfCore
{
    public class EfRepository<T, K>
        : IRepository<T, K> where T : BaseEntity<K> where K : struct, IEquatable<K>
    {
        protected readonly DbContext _context;

        public EfRepository(DbContext context) { _context = context; }

        public async Task<K> CreateAsync(T entity, CancellationToken cancellationToken)
        {
            await _context.AddAsync(entity);

            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task DeleteAsync(K id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);

            if (entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<T> GetAll() => _context.Set<T>();

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(K id, CancellationToken cancellationToken)
        {
            return await _context.FindAsync<T>(id);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            _context.Update(entity);

            await _context.SaveChangesAsync();
        }
    }
}
