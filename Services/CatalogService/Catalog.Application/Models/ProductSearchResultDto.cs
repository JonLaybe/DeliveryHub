namespace Catalog.Application.Models
{
    public class ProductSearchResultDto
    {
        public List<ProductDto> Products { get; init; } = [];
        public Dictionary<Guid, string> Categories { get; init; } = [];
        public Dictionary<string, List<string>> Attributes { get; init; } = [];
    }
}
