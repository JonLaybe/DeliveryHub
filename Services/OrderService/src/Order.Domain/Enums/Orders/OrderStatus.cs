namespace OrderService.Domain.Enums.Orders
{
    public enum OrderStatus
    {
        /// <summary>
        ///  Unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///  Orders that are in the process of being delivered.
        /// </summary>
        Relevant = 1,

        /// <summary>
        ///  Delivered orders.
        /// </summary>
        Completed = 2,

        /// <summary>
        ///  Delivery cancelled.
        /// </summary>
        Cancelled = 3,
    }
}
