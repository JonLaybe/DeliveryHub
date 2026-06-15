using OrderService.Core.Models.Products;
using OrderService.Domain.Enums.Orders;
using System.Text.Json.Serialization;

namespace OrderService.Core.Models.Orders
{
    public class OrderDto
    {
        public long Id { get; set; }

        /// <summary>
        ///  Order status.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
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

        public decimal? Discount { get; set; }

        /// <summary>
        /// Product discount
        /// </summary>
        public decimal? Discount { get; set; }

        /// <summary>
        /// List products in order.
        /// </summary>
        public IList<ProductDto> Products { get; set; }
    }
}
