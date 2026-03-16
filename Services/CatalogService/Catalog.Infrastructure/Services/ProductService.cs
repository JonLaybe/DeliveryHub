using Catalog.Application.Models;
using Catalog.Application.Repositories;
using DeliveryHub.Catalog.Application.Services;
using DeliveryHub.Catalog.Domain.Entities;
using MongoDB.Driver.Linq;
using Shared.Domain.Exceptions;

namespace DeliveryHub.Catalog.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IStockRepository _stockRepository;

        public ProductService(IProductRepository productRepository, IProductImageRepository productImageRepository, IStockRepository stockRepository)
        {
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _stockRepository = stockRepository;
        }

        public async Task<List<ProductDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllAsync(cancellationToken);

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

        public async Task<ProductDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);

            if (product == null)
            {
                throw new NotFoundEntityException(nameof(Product));
            }

            var images = await _productImageRepository.GetAll()
                .Where(x => x.ProductId == product.Id)
                .ToListAsync();

            var stock = await _stockRepository.GetAll().FirstOrDefaultAsync(x => x.ProductId == product.Id);

            return new ProductDto
            {
                Attributes = product.Attributes,
                Description = product.Description,
                Id = product.Id,
                CategoryId = product.CategoryId,
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
