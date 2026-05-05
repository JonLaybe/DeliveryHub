using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Core.Models.Orders;
using OrderService.Core.Services.Interfaces.Orders;

namespace OrderService.WebAPI.Controllers.Orders
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController
    {
        private IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet("{id:int}")]
        public Task<OrderDto> GetOrderAsync(int id, CancellationToken cancellationToken) =>
            this.orderService.GetEntityAsync(id, cancellationToken);

        [HttpGet("GetOrders")]
        public Task<IList<OrderDto>> GetOrdersAsync(CancellationToken cancellationToken) =>
            this.orderService.GetOrdersAsync(cancellationToken);

        [HttpPost("Create")]
        public Task<OrderDto> CreateOrderDto([FromBody] OrderCreateDto orderCreateDto, CancellationToken cancellationToken) =>
            this.orderService.AddAsync(orderCreateDto, cancellationToken);
    }
}
