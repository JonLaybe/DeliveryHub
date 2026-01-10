using OrderService.Domain.Entities.Products;
using OrderService.Domain.Enums.Orders;

namespace OrderService.Core.Models.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }

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
        public DateTime CreatedDate { get; set; }

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
