using Catalog.Infrastructure.Mongo;
using DeliveryHub.Catalog.Domain.Appliaction.Repositories;
using DeliveryHub.CatalogService.Domain.Entities;
using MongoDB.Driver;

namespace DeliveryHub.Catalog.Infrastructure.Repositories
{
    public class CategoryRepository : MongoRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(IMongoDatabase database, string collectionName = "category") : base(database, collectionName)
        {
        }
    }
}
