using Catalog.Application.Repositories;
using Catalog.Infrastructure.Mongo;
using DeliveryHub.Catalog.Domain.Entities;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
    public class StockRepository: MongoRepository<Stock>, IStockRepository
    {
        public StockRepository(IMongoDatabase database, string collectionName = "stock") : base(database, collectionName)
        {

        }
    }
}
