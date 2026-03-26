using OrderService.Domain.Entities.Oriders;

namespace OrderService.Domain.Entities.Products
{
    public class Product : BaseEntity
    {
        /// <summary>
        /// External product ID.
        /// </summary>
        public Guid ArticleNumber { get; set; }

        /// <summary>
        ///  Product quantity.
        /// </summary>
        public int Quantity { get; set; } = 0;

        /// <summary>
        ///  Price product.
        /// </summary>
        public decimal Price { get; set; } = 0;

        /// <summary>
        ///  Link Order.
        /// </summary>
        public long OrderId { get; set; }
        
        public Order Order { get; set; }
    }
}
