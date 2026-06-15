using OrderService.Domain.Entities.Oriders;
using OrderService.Domain.Enums.Orders;

namespace OrderService.Core.Repositories.Interfaces.Orders
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IList<Order>> GetOrdersWithUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<IList<long>> GetOrdersByStateRelevantAsync(DateTime dateTimeNow, CancellationToken cancellationToken = default);

        Task ChangeOrderStateAsync(long orderId, OrderStatus state, CancellationToken cancellationToken = default);
    }
}
