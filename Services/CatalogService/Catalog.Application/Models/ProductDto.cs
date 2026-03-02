namespace Catalog.Application.Models
{
    public class ProductDto
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public decimal Price { get; init; }
        public int AvailableQty { get; init; }
        public Guid CategoryId { get; init; }
        public required IReadOnlyDictionary<string, string> Attributes { get; init; }
        public required IReadOnlyList<ProductImageDto> Images { get; init; }
    }
}
