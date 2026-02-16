
using OrderService.Domain.Entities.Oriders;

namespace OrderService.Core.Models.Products
{
    public class ProductDto
    {
        public int Id { get; set; }

        /// <summary>
        /// External product ID.
        /// </summary>
        public Guid ArticleNumber { get; set; }

        /// <summary>
        ///  Product quantity.
        /// </summary>
        public int Quantity { get; set; } = 0;

        /// <summary>
        ///  Name product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  Price product.
        /// </summary>
        public int Price { get; set; } = 0;

        /// <summary>
        ///  Photo preview Url.
        /// </summary>
        public string PhotoPreviewUrl { get; set; }
    }
}
