namespace DeliveryHub.Catalog.Domain.Entities
{
    public class ProductImage
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public required string Url { get; set; }

        public ProductImageType Type { get; set; }
        public int Order { get; set; }
    }

    public enum ProductImageType
    {
        Main,
        Thumbnail,
        Gallery
    }
}
