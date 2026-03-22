using OrderService.Core.Models.Products;
using OrderService.Domain.Enums.Orders;

namespace OrderService.Core.Models.Orders
{
    public class OrderCreateDto
    {
        /// <summary>
        /// Delivery address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Date of Delivery.
        /// </summary>
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// List products in order.
        /// </summary>
        public IList<ProductCreateDto> Products { get; set; }
    }
}
