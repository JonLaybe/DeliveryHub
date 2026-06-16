namespace DiscountService.Api.Consumers
{
    public class Order
    {
        /// <summary>
        /// Идентификатор заказа.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Discount Usage Id
        /// </summary>
        public int? DiscountUsageId { get; set; }

        /// <summary>
        /// Товары.
        /// </summary>
        public IReadOnlyCollection<OrderProduct> Products { get; set; }
    }
}
