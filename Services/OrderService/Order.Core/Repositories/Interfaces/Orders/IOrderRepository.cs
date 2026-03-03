using OrderService.Domain.Entities.Oriders;

namespace OrderService.Core.Repositories.Interfaces.Orders
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IList<Order>> GetOrdersAsync(CancellationToken cancellationToken);
    }
}
