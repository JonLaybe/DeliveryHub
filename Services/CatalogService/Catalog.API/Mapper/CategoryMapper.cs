using Catalog.API.Contracts;
using DeliveryHub.CatalogService.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Catalog.API.Mapper
{
    [Mapper]
    public partial class CategoryMapper
    {
        public partial CategoryResponseDto MapToDto(Category category);
        public partial List<CategoryResponseDto> MapToDtoList(List<Category> category);
    }
}
