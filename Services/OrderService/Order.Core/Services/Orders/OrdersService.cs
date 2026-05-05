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

            var order = await this.orderRepository.CreateAsync(this.mapper.Map<Order>(entity), cancellationToken);

            await this.orderRepository.SaveChangesAsync(cancellationToken);

            await this.clientRabbitMq.SendMessage(new Shared.Domain.Entities.RabbitMq.Order()
            {
                Id = order.Id,
                CreatedDate = order.CreatedDate,
                Products = order.Products.Select(prd => prd.ArticleNumber).ToList()
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
    }
}
