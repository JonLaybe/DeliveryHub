using OrderService.Core.Models.Orders;
using OrderService.Domain.Entities.Oriders;

namespace OrderService.Core.Services.Interfaces.Orders
{
    public interface IOrderService : ICRUD<OrderDto, OrderCreateDto, OrderUpdateDto>
    {
        Task<IList<OrderDto>> GetOrdersAsync(CancellationToken cancellationToken = default);

        Task UpdateStateByDateAsync(CancellationToken cancellationToken = default);
    }
}
