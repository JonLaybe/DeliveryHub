using DeliveryHub.Catalog.Domain.Entities;

namespace Catalog.Application.Models
{
    public class ProductImageDto
    {
        public Guid ProductId { get; set; }

        public required string Url { get; set; }

        public ProductImageType Type { get; set; }

        public int Order { get; set; }
    }
}
