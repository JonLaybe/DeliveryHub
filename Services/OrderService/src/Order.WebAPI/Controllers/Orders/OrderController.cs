using Microsoft.AspNetCore.Mvc;
using OrderService.Core.Models.Orders;
using OrderService.Core.Services.Interfaces.Orders;

namespace OrderService.WebAPI.Controllers.Orders
{
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

        [HttpPost("Create")]
        public Task<OrderDto> CreateOrderDto([FromBody] OrderCreateDto orderCreateDto, CancellationToken cancellationToken) =>
            this.orderService.AddAsync(orderCreateDto, cancellationToken);
    }
}
