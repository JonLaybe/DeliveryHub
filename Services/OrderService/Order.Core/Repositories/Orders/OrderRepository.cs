using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Core.Common.Exceptions;
using OrderService.Core.Common.Interfaces;
using OrderService.Core.Repositories.Interfaces.Orders;
using OrderService.Domain.Entities.Oriders;
using OrderService.Domain.Enums.Orders;

namespace OrderService.Core.Repositories.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IApplicationDbContext applicationDbContext;
        private readonly ILogger<OrderRepository> logger;

        public OrderRepository(
            IApplicationDbContext applicationDbContext,
            ILogger<OrderRepository> logger)
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
        }

        public async Task<Order> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var order = await this.applicationDbContext.Orders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (order == default)
            {
                this.logger.LogError("[GetByIdAsync] Error: Order not found.");
                throw new NotFoundEntityException(nameof(Order));
            }

            return order;
        }

        public async Task<IList<Order>> GetOrdersWithUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var orders = await this.applicationDbContext.Orders.Where(ord => ord.UserId == userId)
                .Include(x => x.Products).ToListAsync(cancellationToken);

            return orders;
        }

        public async Task<Order> CreateAsync(Order entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                this.logger.LogError("[CreateAsync] Error: Order not found.");
                throw new ArgumentNullException(nameof(entity));
            }

            _ = await this.applicationDbContext.Orders.AddAsync(entity);

            return entity;
        }

        public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var order = await this.applicationDbContext.Orders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (order == default)
            {
                this.logger.LogError("[DeleteAsync] Error: Order not found.");
                throw new NotFoundEntityException(nameof(Order));
            }

            this.applicationDbContext.Orders.Remove(order);

            return id;
        }

        public async Task UpdateAsync(Order entity, CancellationToken cancellationToken = default)
        {
            var orderUpdate = await this.applicationDbContext.Orders.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

            if (orderUpdate == default)
            {
                this.logger.LogError("[UpdateAsync] Error: Order not found.");
                throw new NotFoundEntityException(nameof(Order));
            }

            orderUpdate.Status = entity.Status;
            orderUpdate.Address = entity.Address;
            orderUpdate.DeliveryDate = entity.DeliveryDate;
            orderUpdate.Products = entity.Products;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await this.applicationDbContext.SaveChangesAsync(cancellationToken);

        public async Task<IList<long>> GetOrdersByStateRelevantAsync(DateTime dateTimeNow, CancellationToken cancellationToken = default) =>
            await this.applicationDbContext.Orders.Where(x => dateTimeNow >= x.DeliveryDate
            && x.Status == OrderStatus.Relevant)
            .Select(x => x.Id)
            .ToListAsync();

        public async Task ChangeOrderStateAsync(long orderId, OrderStatus state, CancellationToken cancellationToken = default)
        {
            var order = await this.applicationDbContext.Orders.Where(ord => ord.Id == orderId).FirstOrDefaultAsync(cancellationToken);

            if (order == null)
            {
                this.logger.LogError("[UpdateAsync] Error: Order not found.");
                throw new NotFoundEntityException(nameof(Order));
            }

            order.Status = state;

            await this.SaveChangesAsync(cancellationToken);
        }
    }
}
