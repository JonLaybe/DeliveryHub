using Shared.Domain.Entities;

namespace DeliveryHub.Catalog.Domain.Entities
{
    public class Stock: BaseEntity<Guid>
    {
        public Guid ProductId { get; set; }
        public int TotalQty { get; set; }
        public int ReservedQty { get; set; }

        public int AvailableQty => TotalQty - ReservedQty;
    }

}
