using Catalog.Application.Models;

namespace Catalog.API.Contracts
{
    public record ProductResponseDto
        (Guid id, string Name, string Description, decimal Price, int AvailableQty, Guid CategoryId, Guid SellerId,
        IReadOnlyDictionary<string, string> Attributes, IReadOnlyList<ProductImageDto> Images);
}
