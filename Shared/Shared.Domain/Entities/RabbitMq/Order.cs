namespace Shared.Domain.Entities.RabbitMq
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
        /// Идентификатор продуктов, которые оформили в заказ.
        /// </summary>
        public IReadOnlyCollection<Guid> Products { get; set; }
    }
}
