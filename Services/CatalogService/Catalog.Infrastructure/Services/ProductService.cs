using Catalog.Application.Models;
using Catalog.Application.Repositories;
using Catalog.Infrastructure.Mongo;
using DeliveryHub.Catalog.Application.Services;
using DeliveryHub.Catalog.Domain.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Shared.Domain.Exceptions;

namespace DeliveryHub.Catalog.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IStockRepository _stockRepository;
        private readonly MongoDatabase _mongoDB;

        public ProductService(IProductRepository productRepository, IProductImageRepository productImageRepository, IStockRepository stockRepository, MongoDatabase mongo)
        {
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _stockRepository = stockRepository;
            _mongoDB = mongo;
        }

        private async Task<Dictionary<string, string>> GetAttributesAsync(Product product)
        {
            var productAttributesDict = await _mongoDB.ProductAttributes.AsQueryable()
                .ToListAsync();

            var attributes = new Dictionary<string, string>();
            foreach (var attr in product.Attributes)
            {
                var attrFromDict = productAttributesDict.FirstOrDefault(s => s.Key == attr.Key);
                var key = attrFromDict?.Name ?? attr.Key;
                var value = attrFromDict?.Values.FirstOrDefault(v => v.Value == attr.Value)?.Label ?? attr.Value;
                attributes.Add(key, value);
            }

            return attributes;
        }

        private async Task<List<ProductDto>> DressProductsToDtoAsync(IEnumerable<Product> products)
        {
            var productIds = products.Select(s => s.Id);

            var stocks = await _stockRepository.GetAll()
                .Where(x => productIds.Contains(x.ProductId))
                .ToListAsync();

            var images = await _productImageRepository.GetAll()
                .Where(x => productIds.Contains(x.ProductId))
                .ToListAsync();

            var stocksDict = stocks.ToDictionary(k => k.ProductId, v => v);

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

            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                Attributes = p.Attributes,
                AvailableQty = stocksDict.GetValueOrDefault(p.Id)?.AvailableQty ?? 0,
                Images = imagesDict.GetValueOrDefault(p.Id) ?? []
            }).ToList();
        }

        public async Task<List<ProductDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllAsync(cancellationToken);

            return await DressProductsToDtoAsync(products);
        }

        public async Task<List<ProductDto>> GetByManyIdAsync(List<Guid> ids, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAll()
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();

            return await DressProductsToDtoAsync(products);
        }

        public async Task<ProductDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);

            if (product == null)
            {
                throw new NotFoundEntityException(nameof(Product));
            }

            var attributes = await GetAttributesAsync(product);

            var images = await _productImageRepository.GetAll()
                .Where(x => x.ProductId == product.Id)
                .ToListAsync();

            var stock = await _stockRepository.GetAll().FirstOrDefaultAsync(x => x.ProductId == product.Id);

            return new ProductDto
            {
                Attributes = attributes,
                Description = product.Description,
                Id = product.Id,
                CategoryId = product.CategoryId,
                SellerId = product.SellerId,
                Name = product.Name,
                Price = product.Price,
                AvailableQty = stock?.AvailableQty ?? 0,
                Images = images?.Select(i => new ProductImageDto
                {
                    Url = i.Url,
                    Order = i.Order,
                    ProductId = i.ProductId,
                    Type = i.Type,
                }).ToList().AsReadOnly()
            };
        }
    }
}
