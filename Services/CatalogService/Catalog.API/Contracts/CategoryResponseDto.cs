namespace Catalog.API.Contracts
{
    public record CategoryResponseDto(Guid Id, string Name, Guid? ParentId);
}
