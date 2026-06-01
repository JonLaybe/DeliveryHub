namespace Chat.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public decimal Price { get; init; }
        public Guid SellerId { get; set; }
    }
}
