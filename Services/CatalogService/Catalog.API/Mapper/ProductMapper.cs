using Catalog.API.Contracts;
using Catalog.Application.Models;
using Riok.Mapperly.Abstractions;

namespace Catalog.API.Mapper
{
    [Mapper]
    public partial class ProductMapper
    {
        public partial ProductResponseDto Map(ProductDto productDto);

        public partial ProductSearchQuery Map(ProductSearchQueryRequest productSearchQueryRequest);
    }
}
