using Catalog.Domain.Entities;
using DeliveryHub.Catalog.Domain.Entities;
using DeliveryHub.CatalogService.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Shared.Domain.Entities;

namespace Catalog.Infrastructure.Mongo
{
    public static class MongoMappings
    {
        public static void Register()
        {
            // Set to Standard (Binary Subtype 4)
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            if (!BsonClassMap.IsClassMapRegistered(typeof(BaseEntity<Guid>)))
            {
                BsonClassMap.RegisterClassMap<BaseEntity<Guid>>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(x => x.Id);
                    cm.SetIsRootClass(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Product)))
            {
                BsonClassMap.RegisterClassMap<Product>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Category)))
            {
                BsonClassMap.RegisterClassMap<Category>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(ProductImage)))
            {
                BsonClassMap.RegisterClassMap<ProductImage>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Stock)))
            {
                BsonClassMap.RegisterClassMap<Stock>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(ProductAttribute)))
            {
                BsonClassMap.RegisterClassMap<ProductAttribute>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}
