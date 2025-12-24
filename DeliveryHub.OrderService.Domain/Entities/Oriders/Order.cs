using DeliveryHub.OrderService.Domain.Entities.Products;
using DeliveryHub.OrderService.Domain.Enums.Orders;

namespace DeliveryHub.OrderService.Domain.Entities.Oriders
{
    public class Order : BaseEntity
    {
        /// <summary>
        ///  External order ID.
        /// </summary>
        public Guid OrderNumber { get; set; } = Guid.NewGuid();

        /// <summary>
        ///  Product quantity.
        /// </summary>
        public int Quantity { get; set; } = 0;

        /// <summary>
        ///  Order status.
        /// </summary>
        public OrderStatus Status { get; set; } = OrderStatus.Unknown;

        /// <summary>
        /// Delivery address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The date and time when the order was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date of Delivery.
        /// </summary>
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// List products in order.
        /// </summary>
        public IList<Product> Products { get; set; }
    }
}
