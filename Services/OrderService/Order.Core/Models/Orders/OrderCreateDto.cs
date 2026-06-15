using OrderService.Core.Models.Products;
using OrderService.Domain.Enums.Orders;

namespace OrderService.Core.Models.Orders
{
    public class OrderCreateDto
    {
        public Guid UserId { get; set; }

        /// <summary>
        /// Delivery address.
        /// </summary>
        public string Address { get; set; }

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
        /// Discount Usage Id
        /// </summary>
        public int? DiscountUsageId { get; set; }

        /// <summary>
        /// List products in order.
        /// </summary>
        public IList<ProductCreateDto> Products { get; set; }
    }
}
