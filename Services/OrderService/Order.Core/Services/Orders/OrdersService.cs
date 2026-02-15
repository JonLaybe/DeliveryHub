using AutoMapper;
using OrderService.Core.Models.Orders;
using OrderService.Core.Repositories.Interfaces.Orders;
using OrderService.Core.Services.Interfaces.Orders;
using OrderService.Domain.Entities.Oriders;

namespace OrderService.Core.Services.Orders
{
    public class OrdersService : IOrderService
    {
        private IOrderRepository orderRepository;
        private IMapper mapper;

        public OrdersService(
            IOrderRepository repository,
            IMapper mapper)
        {
            this.orderRepository = repository;
            this.mapper = mapper;
        }

        public async Task<OrderDto> GetEntityAsync(int id, CancellationToken cancellationToken = default)
        {
            var order = await this.orderRepository.GetByIdAsync(id, cancellationToken);

            return this.mapper.Map<OrderDto>(order);
        }

        public async Task<IList<OrderDto>> GetOrdersAsync(CancellationToken cancellationToken = default)
        {
            var orders = (await this.orderRepository.GetOrdersAsync(cancellationToken)).Select(ord => this.mapper.Map<OrderDto>(ord)).ToList();

            return orders;
        }

        public async Task<OrderDto> AddAsync(OrderCreateDto entity, CancellationToken cancellationToken = default)
        {
            var order = await this.orderRepository.CreateAsync(this.mapper.Map<Order>(entity), cancellationToken);

            await this.orderRepository.SaveChangesAsync(cancellationToken);

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
