using OrderService.Domain.Entities.Products;

namespace OrderService.Core.Repositories.Interfaces.Products
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> GetProductByArticul(Guid articul, CancellationToken cancellationToken);
    }
}
