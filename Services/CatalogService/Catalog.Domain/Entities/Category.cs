using Shared.Domain.Entities;

namespace DeliveryHub.CatalogService.Domain.Entities
{
    public class Category : BaseEntity<Guid>
    {
        public Category() { }

        public string Name { get; set; } = default!;

        public Guid? ParentId { get; set; }
    }
}
