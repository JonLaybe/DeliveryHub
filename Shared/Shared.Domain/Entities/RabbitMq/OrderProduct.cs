namespace Shared.Domain.Entities.RabbitMq
{
    public class OrderProduct
    {
        public Guid Id { get; set; }

        /// <summary>
        ///  Количество товара.
        /// </summary>
        public int Quantity { get; set; } = 0;

        /// <summary>
        /// Цена товара.
        /// </summary>
        public decimal Price { get; set; } = 0;
    }
}
