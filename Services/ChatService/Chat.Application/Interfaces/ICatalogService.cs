using Chat.Application.DTOs;

namespace Chat.Application.Interfaces
{
    public interface ICatalogService
    {
        Task<ProductDto?> GetProductByIdAsync(Guid productId);
    }
}
