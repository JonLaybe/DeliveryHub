using Catalog.Application.Models;

namespace Catalog.Application.Services
{
    public interface IProductSearchService
    {
        Task<IEnumerable<ProductDto>> SearchAsync(ProductSearchQuery searchQuery, CancellationToken cancellationToken);

        Task<IEnumerable<string>> SuggestAsync(string query, CancellationToken cancellationToken);
    }
}
