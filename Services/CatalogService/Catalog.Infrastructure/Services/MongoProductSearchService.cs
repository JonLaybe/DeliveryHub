using Catalog.Application.Models;
using Catalog.Application.Services;
using Catalog.Infrastructure.Mongo;
using DeliveryHub.Catalog.Domain.Appliaction.Repositories;
using DeliveryHub.Catalog.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Catalog.Infrastructure.Services
{
    public class MongoProductSearchService : IProductSearchService
    {
        private MongoDatabase _database;
        private readonly ICategoryRepository _categoryRepository;

        public MongoProductSearchService(MongoDatabase database, ICategoryRepository categoryRepository)
        {
            _database = database;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<string>> SuggestAsync(string query, CancellationToken cancellationToken)
        {
            var normalizedQuery = query.ToLower();
            var filter = Builders<Product>.Filter.AnyEq(p => p.SearchTokens, normalizedQuery);

            var suggestions = await _database.Products
                .Find(filter)
                .Limit(10)
                .Project(p => p.Name)
                .ToListAsync(cancellationToken);

            if (!suggestions.Any())
            {
                var regex = new BsonRegularExpression($"^{normalizedQuery}", "i");

                var regexFilter = Builders<Product>.Filter.Regex(
                    p => p.Name,
                    regex
                );

                suggestions = await _database.Products
                    .Find(regexFilter)
                    .Limit(10)
                    .Project(p => p.Name)
                    .ToListAsync();
            }

            return suggestions;
        }

        public async Task<ProductSearchResultDto> SearchAsync(ProductSearchQuery searchQuery, CancellationToken cancellationToken)
        {
            var filter = Builders<Product>.Filter.Empty;
            List<Product> matchExact = new();

            if (!string.IsNullOrWhiteSpace(searchQuery.Text))
            {
                var exactFilter = Builders<Product>.Filter.Eq(p => p.Name, searchQuery.Text);
                matchExact = await _database.Products.Find(exactFilter).ToListAsync();

                //filter &= Builders<Product>.Filter.Text(searchQuery.Text);
                filter &= Builders<Product>.Filter.AnyEq(p => p.SearchTokens, searchQuery.Text.ToLower());
            }

            if (searchQuery.CategoryId.HasValue)
            {
                var allCategories = await _categoryRepository.GetAll().ToListAsync();

                var subCategories = allCategories
                    .Where(x => x.ParentId == searchQuery.CategoryId.Value)
                    .Select(x => x.Id);

                if (subCategories.Any())
                {
                    filter &= Builders<Product>.Filter
                        .In(p => p.CategoryId, subCategories);
                }
                else
                {
                    filter &= Builders<Product>
                        .Filter.Eq(p => p.CategoryId, searchQuery.CategoryId.Value);
                }
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
                foreach (var (key, values) in searchQuery.Attributes.Where(w => w.Key != "minPrice" && w.Key != "maxPrice"))
                {
                    filter &= Builders<Product>.Filter.In($"Attributes.{key}", values);
                }
            }

            var sorting = Builders<Product>.Sort.Ascending(x => x.Price);

            if (searchQuery.Sort == SortOption.PriceDesc)
            {
                sorting = sorting.Descending(x => x.Price);
            }

            List<Product> products = new();

            if (matchExact.Any())
            {
                products = matchExact;
            }
            else
            {
                var match = await _database.Products.FindAsync(filter, 
                    options: new FindOptions<Product, Product>() { Sort = sorting }, 
                    cancellationToken: cancellationToken);

                products = match.ToList();
            }

            var productIds = products.Select(x => x.Id).ToList();

            var images = await _database.ProductImages.FindAsync(x => productIds.Contains(x.ProductId));

            var stocks = await _database.Stock.FindAsync(x => productIds.Contains(x.ProductId));

            var stocksDict = stocks.ToList().ToDictionary(k => k.ProductId, v => v);

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

            //var queryCategories = allCategories.Where(x => products.Select(p => p.CategoryId).Contains(x.Id));
            var attributes = products.SelectMany(p => p.Attributes).GroupBy(a => a.Key)
                .ToDictionary(k => k.Key, v => v.Select(s => s.Value)
                .Distinct().ToList());

            var result = new ProductSearchResultDto
            {
                Attributes = attributes,
                //Categories = queryCategories,
                Products = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    Attributes = p.Attributes,
                    AvailableQty = stocksDict.GetValueOrDefault(p.Id)?.AvailableQty ?? 0,
                    Images = imagesDict.GetValueOrDefault(p.Id) ?? [],
                }).ToList()
            };

            return result;
        }
    }
}
