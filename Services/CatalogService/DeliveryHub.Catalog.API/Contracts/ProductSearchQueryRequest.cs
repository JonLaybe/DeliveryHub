using Catalog.Application.Models;

namespace Catalog.API.Contracts
{
    public class ProductSearchQueryRequest
    {
        // Full-text
        public string? Text { get; init; }

        // Категория
        public Guid? CategoryId { get; init; }

        // Цена
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }

        // Динамические атрибуты
        /// <summary>
        /// attributes[key]=value
        /// Example: attributes[color]=red&attributes[size]=xl
        /// </summary>
        public Dictionary<string, string>? Attributes { get; init; }

        // Пагинация
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;

        // Сортировка
        public SortOption Sort { get; init; } = SortOption.Relevance;
    }
}
