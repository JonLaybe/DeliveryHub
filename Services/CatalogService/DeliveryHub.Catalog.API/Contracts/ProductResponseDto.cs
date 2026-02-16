namespace Catalog.API.Contracts
{
    public record ProductResponseDto(Guid id, string Name, string Description, decimal Price, Guid CategoryId, IReadOnlyDictionary<string, string> Attributes);
}
