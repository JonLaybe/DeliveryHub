using Shared.Domain.Entities;

namespace DeliveryHub.Catalog.Domain.Entities
{
    public class Product: BaseEntity<Guid>
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }

        public Guid CategoryId { get; set; }

        public Guid SellerId { get; set; }

        // Динамические характеристики
        public Dictionary<string, string> Attributes { get; set; } = [];

        public List<string> SearchTokens { get; set; } = [];
    }

}
