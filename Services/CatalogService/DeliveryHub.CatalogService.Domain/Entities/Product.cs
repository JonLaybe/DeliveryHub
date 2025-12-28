namespace DeliveryHub.Catalog.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }

        public Guid CategoryId { get; set; }

        // Динамические характеристики
        public Dictionary<string, string> Attributes { get; set; } = [];
    }

}
