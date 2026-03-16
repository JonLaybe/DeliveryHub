using Catalog.Application.Repositories;
using Catalog.Infrastructure.Mongo;
using DeliveryHub.Catalog.Domain.Entities;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository: MongoRepository<Product>, IProductRepository
    {
        public ProductRepository(IMongoDatabase database, string collectionName = "product") : base(database, collectionName)
        {
        }
    }
}
