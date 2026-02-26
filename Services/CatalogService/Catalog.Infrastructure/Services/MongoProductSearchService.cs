using Catalog.Application.Models;
using Catalog.Application.Services;
using DeliveryHub.Catalog.Domain.Entities;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Services
{
    public class MongoProductSearchService : IProductSearchService
    {
        private IMongoCollection<Product> _productsCollection;
        private IMongoCollection<ProductImage> _productImagesCollection;

        public MongoProductSearchService(IMongoDatabase database)
        {
            _productsCollection = database.GetCollection<Product>("product");
            _productImagesCollection = database.GetCollection<ProductImage>("product_image");
        }

        public async Task<IEnumerable<ProductDto>> SearchAsync(ProductSearchQuery searchQuery, CancellationToken cancellationToken)
        {
            var filter = Builders<Product>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(searchQuery.Text))
            {
                filter &= Builders<Product>.Filter.Text(searchQuery.Text);
            }

            if (searchQuery.CategoryId.HasValue)
            {
                filter &= Builders<Product>
                    .Filter.Eq(p => p.CategoryId, searchQuery.CategoryId.Value);
            }

            if (searchQuery.MinPrice.HasValue)
            {
                filter &= Builders<Product>
                    .Filter.Gte(p => p.Price, searchQuery.MinPrice.Value);
            }

            if (searchQuery.MaxPrice.HasValue)
            {
                filter &= Builders<Product>
                    .Filter.Lte(p => p.Price, searchQuery.MaxPrice.Value);
            }

            if (searchQuery.Attributes is not null)
            {
                foreach (var (key, value) in searchQuery.Attributes)
                {
                    filter &= Builders<Product>
                        .Filter.Eq($"Attributes.{key}", value);
                }
            }

            var match = await _productsCollection.FindAsync(filter, cancellationToken: cancellationToken);

            var result = match.ToList();
            var productIds = result.Select(x => x.Id).ToList();

            var images = await _productImagesCollection.FindAsync(x => productIds.Contains(x.Id));

            var imagesDict = images.ToList()
                .GroupBy(x => x.ProductId)
                .ToDictionary(
                    k => k.Key,
                    v => v.Select(s => new ProductImageDto
                    {
                        ProductId = s.ProductId,
                        Url = s.Url,
                        Order = s.Order,
                        Type = s.Type,
                    }).ToList());

            return result.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                Attributes = p.Attributes,
                Images = imagesDict.GetValueOrDefault(p.Id) ?? []
            });
        }
    }
}
