using OrderService.Core.Models.Products;

namespace OrderService.Core.Services.Interfaces.Products
{
    public interface IProductService
    {
        Task<ProductDto> AddAsync(int orderId, ProductCreateDto entity, CancellationToken cancellationToken = default);
    }
}
