using DeliveryHub.Catalog.Domain.Entities;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Mongo
{
    public class MongoDatabase
    {
        private readonly IMongoDatabase _database;
        public MongoDatabase(IMongoDatabase database)
        {
            _database = database;
        }

        public IMongoCollection<Product> Products => _database.GetCollection<Product>("product");
        public IMongoCollection<ProductImage> ProductImages => _database.GetCollection<ProductImage>("product_image");
        public IMongoCollection<Stock> Stock => _database.GetCollection<Stock>("stock");

        public async Task CreateIndexesAsync()
        {
            await Products.Indexes.CreateManyAsync(new[]
            {
                new CreateIndexModel<Product>(
                    Builders<Product>.IndexKeys
                    .Ascending("Attributes.$**"),
                    new CreateIndexOptions { Name = "idx_products_attributes_wildcard" }),

                new CreateIndexModel<Product>(
                    Builders<Product>.IndexKeys
                    .Text(p => p.Name)
                    .Text(p => p.Description),
                    new CreateIndexOptions { Name = "idx_products_name_description" }),

                new CreateIndexModel<Product>(
                    Builders<Product>.IndexKeys
                    .Ascending(x => x.CategoryId)
                    .Ascending(x => x.Price),
                    new CreateIndexOptions { Name = "idx_products_categoryId_price" }),

                new CreateIndexModel<Product>(
                    Builders<Product>.IndexKeys
                    .Ascending(p => p.SearchTokens),
                    new CreateIndexOptions { Name = "idx_products_search_tokens" })
            });

            await Stock.Indexes.CreateOneAsync(new CreateIndexModel<Stock>(
                Builders<Stock>.IndexKeys.Ascending(x => x.ProductId),
                new CreateIndexOptions { Name = "idx_stock_productId" }));

            await ProductImages.Indexes.CreateOneAsync(new CreateIndexModel<ProductImage>(
                Builders<ProductImage>.IndexKeys.Ascending(x => x.ProductId),
                new CreateIndexOptions { Name = "idx_product_image_productId" }));
        }

    }
}
