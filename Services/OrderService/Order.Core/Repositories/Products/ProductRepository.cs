using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Core.Common.Exceptions;
using OrderService.Core.Common.Interfaces;
using OrderService.Core.Repositories.Interfaces.Products;
using OrderService.Domain.Entities.Products;

namespace OrderService.Core.Repositories.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly IApplicationDbContext applicationDbContext;
        private readonly ILogger<ProductRepository> logger;

        public ProductRepository(
            IApplicationDbContext applicationDbContext,
            ILogger<ProductRepository> logger)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
        }

        public Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Product> GetProductByArticul(Guid articul, CancellationToken cancellationToken)
        {
            var productByArticul = await this.applicationDbContext.Products.FirstOrDefaultAsync(prd => prd.ArticleNumber == articul);

            if (productByArticul == null)
            {
                this.logger.LogError("[GetProductByArticul] Error: product not found.");
                throw new NotFoundEntityException(nameof(Product));
            }

            return productByArticul;
        }

        public async Task<Product> CreateAsync(Product entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                this.logger.LogError("[CreateAsync] Error: Product is null.");
                throw new ArgumentNullException(nameof(entity));
            }

            _ = await this.applicationDbContext.Products.AddAsync(entity);

            return entity;
        }

        public Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await this.applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}
