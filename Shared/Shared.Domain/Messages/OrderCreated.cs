namespace Shared.Domain.Messages
{
    public class OrderCreated
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
        /// Идентификатор продуктов и их количество которые оформили в заказ.
        /// </summary>
        public IReadOnlyDictionary<Guid, int> ProductsWithQty { get; set; }
    }
}
