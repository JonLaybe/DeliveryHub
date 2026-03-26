using Catalog.Application.Models;

namespace DeliveryHub.Catalog.Application.Services
{
    public interface IProductService
    {
        Task<ProductDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<ProductDto>> GetByManyIdAsync(List<Guid> ids, CancellationToken cancellationToken);
        Task<List<ProductDto>> GetAllAsync(CancellationToken cancellationToken);
    }
}
