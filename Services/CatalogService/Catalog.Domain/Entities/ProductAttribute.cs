namespace Catalog.Domain.Entities
{
    public class ProductAttribute
    {
        public string Key { get; set; } = default!;   // color

        public string Name { get; set; } = default!;  // Цвет

        public List<ProductAttributeValue> Values { get; set; } = new();
    }

    public class ProductAttributeValue
    {
        public string Value { get; set; } = default!; // pink

        public string Label { get; set; } = default!; // Розовый
    }
}
