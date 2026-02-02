using DeliveryHub.Abstractions.DataAccess;
using DeliveryHub.Domain.Entities;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Mongo
{
    public class MongoRepository<T> : IRepository<T, Guid> where T : BaseEntity<Guid>
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task<Guid> CreateAsync(T entity, CancellationToken cancellationToken)
        {
            await _collection.InsertOneAsync(entity);
            return entity.Id;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await _collection.DeleteOneAsync(e => e.Id == id);
        }

        public IQueryable<T> GetAll(CancellationToken cancellationToken)
        {
            return _collection.AsQueryable();
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
        }
    }
}
