using OrderService.Domain.Entities.Oriders;
using Shared.Abstraction.Intarfaces;

namespace OrderService.Core.Repositories.Interfaces.Orders
{
    public interface IOrderRepository : IRepository<Order>
    {
    }
}
