using Microsoft.EntityFrameworkCore;
using OrderService.Core.Common.Exceptions;
using OrderService.Core.Common.Interfaces;
using OrderService.Core.Repositories.Interfaces.Products;
using OrderService.Domain.Entities.Products;

namespace OrderService.Core.Repositories.Products
{
    public class ProductRepository : IProductRepository
    {
        private IApplicationDbContext applicationDbContext;

        public ProductRepository(IApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Product> GetProductByArticul(Guid articul, CancellationToken cancellationToken)
        {
            var productByArticul = await this.applicationDbContext.Products.FirstOrDefaultAsync(prd => prd.ArticleNumber == articul);

            if (productByArticul == null)
                throw new NotFoundEntityException(nameof(Product));

            return productByArticul;
        }

        public async Task<Product> CreateAsync(Product entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

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
