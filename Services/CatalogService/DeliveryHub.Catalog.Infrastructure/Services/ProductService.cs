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

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
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

            return new ProductDto
            {
                Attributes = product.Attributes,
                Description = product.Description,
                Id = product.Id,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Price = product.Price,
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
