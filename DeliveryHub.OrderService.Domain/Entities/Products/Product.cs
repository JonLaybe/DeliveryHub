using DeliveryHub.OrderService.Domain.Entities.Oriders;

namespace DeliveryHub.OrderService.Domain.Entities.Products
{
    public class Product : BaseEntity
    {
        /// <summary>
        /// External product ID.
        /// </summary>
        public Guid ArticleNumber { get; set; }

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

        /// <summary>
        ///  Link Order.
        /// </summary>
        public int OrderId { get; set; }
        
        public Order Order { get; set; }
    }
}
