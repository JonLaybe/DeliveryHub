using DeliveryHub.Catalog.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace Catalog.Infrastructure.Mongo
{
    public static class MongoMappings
    {
        public static void Register()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Product)))
            {
                BsonClassMap.RegisterClassMap<Product>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(x => x.Id);
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}
