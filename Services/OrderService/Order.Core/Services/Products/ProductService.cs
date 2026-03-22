using AutoMapper;
using OrderService.Core.Models.Products;
using OrderService.Core.Repositories.Interfaces.Products;
using OrderService.Core.Services.Interfaces.Products;

namespace OrderService.Core.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly IMapper mapper;

        public ProductService(
            IProductRepository productRepository,
            IMapper mapper)
        {
            this.productRepository = productRepository;
            this.mapper = mapper;
        }

        public async Task<ProductDto> AddAsync(int orderId, ProductCreateDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException();

            var newProduct = await this.productRepository.CreateAsync(new Domain.Entities.Products.Product
            {
                OrderId = orderId,
                ArticleNumber = entity.ArticleNumber,
                Price = entity.Price,
                Quantity = entity.Quantity,
            });

            await this.productRepository.SaveChangesAsync(cancellationToken);

            return this.mapper.Map<ProductDto>(newProduct);
        }
    }
}
