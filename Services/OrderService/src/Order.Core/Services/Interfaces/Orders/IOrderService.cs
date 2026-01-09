using OrderService.Core.Models.Orders;

namespace OrderService.Core.Services.Interfaces.Orders
{
    public interface IOrderService : ICRUD<OrderDto, OrderCreateDto, OrderUpdateDto>
    {
    }
}
