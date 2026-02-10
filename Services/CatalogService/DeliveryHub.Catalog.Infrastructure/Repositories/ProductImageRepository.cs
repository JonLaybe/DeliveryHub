using Catalog.Application.Repositories;
using Catalog.Infrastructure.Mongo;
using DeliveryHub.Catalog.Domain.Entities;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductImageRepository : MongoRepository<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(IMongoDatabase database, string collectionName = "product_image") : base(database, collectionName)
        {
        }
    }
}
