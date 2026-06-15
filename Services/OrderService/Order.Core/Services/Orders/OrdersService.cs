using AutoMapper;
using OrderService.Core.Models.Orders;
using OrderService.Core.Repositories.Interfaces.Orders;
using OrderService.Core.Services.Interfaces.Orders;
using OrderService.Core.Services.Interfaces.Users;
using OrderService.Domain.Entities.Oriders;
using Shared.RabbitMq.Interfaces;

namespace OrderService.Core.Services.Orders
{
    public class OrdersService : IOrderService
    {
        private readonly IOrderRepository orderRepository;
        private readonly IClientRabbitMq clientRabbitMq;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public OrdersService(
            IOrderRepository repository,
            IClientRabbitMq clientRabbitMq,
            IUserService userService,
            IMapper mapper)
        {
            this.orderRepository = repository;
            this.clientRabbitMq = clientRabbitMq;
            this.userService = userService;
            this.mapper = mapper;
        }

        public async Task<OrderDto> GetEntityAsync(int id, CancellationToken cancellationToken = default)
        {
            var order = await this.orderRepository.GetByIdAsync(id, cancellationToken);

            return this.mapper.Map<OrderDto>(order);
        }

        public async Task<IList<OrderDto>> GetOrdersAsync(CancellationToken cancellationToken = default)
        {
            var userId = this.userService.GetCurrentUserId();

            var orders = (await this.orderRepository.GetOrdersWithUserIdAsync(userId, cancellationToken)).Select(ord => this.mapper.Map<OrderDto>(ord)).ToList();

            return orders;
        }

        public async Task<OrderDto> AddAsync(OrderCreateDto entity, CancellationToken cancellationToken = default)
        {
            var userId = this.userService.GetCurrentUserId();
            entity.UserId = userId;

            var newOrder = this.mapper.Map<Order>(entity);
            newOrder.Status = Domain.Enums.Orders.OrderStatus.Relevant;

            var order = await this.orderRepository.CreateAsync(newOrder, cancellationToken);

            await this.orderRepository.SaveChangesAsync(cancellationToken);

            await this.clientRabbitMq.SendMessage(new Shared.Domain.Entities.RabbitMq.Order()
            {
                Id = order.Id,
                CreatedDate = order.CreatedDate,
                Products = order.Products.Select(prd => new Shared.Domain.Entities.RabbitMq.OrderProduct()
                {
                    Id = prd.ArticleNumber,
                    Price = prd.Price,
                    Quantity = prd.Quantity,
                }).ToList()
            });

            return this.mapper.Map<OrderDto>(order);
        }

        public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            _ = await this.orderRepository.DeleteAsync(id, cancellationToken);

            await this.orderRepository.SaveChangesAsync(cancellationToken);

            return id;
        }

        public async void UpdateAsync(OrderUpdateDto entity, CancellationToken cancellationToken = default)
        {
            await this.orderRepository.UpdateAsync(this.mapper.Map<Order>(entity), cancellationToken);

            await this.orderRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateStateByDateAsync(CancellationToken cancellationToken = default)
        {
            var moscowNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));

            var orders = await this.orderRepository.GetOrdersByStateRelevantAsync(moscowNow, cancellationToken);
            foreach (var orderId in orders)
                await this.orderRepository.ChangeOrderStateAsync(orderId, Domain.Enums.Orders.OrderStatus.Completed, cancellationToken);
        }
    }
}
