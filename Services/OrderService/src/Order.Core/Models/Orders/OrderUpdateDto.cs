using OrderService.Domain.Entities.Products;
using OrderService.Domain.Enums.Orders;

namespace OrderService.Core.Models.Orders
{
    public class OrderUpdateDto
    {
        public int Id { get; set; }

        /// <summary>
        /// Delivery address.
        /// </summary>
        public string Address { get; set; }
    }
}
