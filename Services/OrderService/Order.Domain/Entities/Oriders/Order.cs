using OrderService.Domain.Entities.Products;
using OrderService.Domain.Enums.Orders;

namespace OrderService.Domain.Entities.Oriders
{
    public class Order : BaseEntity
    {
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
