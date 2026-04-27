using OrderService.Domain.Entities.Oriders;

namespace OrderService.Core.Repositories.Interfaces.Orders
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetByIdWithUserIdAsync(int id, Guid userId, CancellationToken cancellationToken = default);

        Task<IList<Order>> GetOrdersWithUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
