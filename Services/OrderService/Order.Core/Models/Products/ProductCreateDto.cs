namespace OrderService.Core.Models.Products
{
    public class ProductCreateDto
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
    }
}
