using Microsoft.EntityFrameworkCore;
using OrderService.Core.Common.Exceptions;
using OrderService.Core.Common.Interfaces;
using OrderService.Core.Repositories.Interfaces.Orders;
using OrderService.Domain.Entities.Oriders;

namespace OrderService.Core.Repositories.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private IApplicationDbContext applicationDbContext;

        public OrderRepository(IApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<Order> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var order = await this.applicationDbContext.Orders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (order == default)
                throw new NotFoundEntityException(nameof(Order));

            return order;
        }

        public Task<Order> GetByIdWithUserIdAsync(int id, Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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
                throw new ArgumentNullException(nameof(entity));

            _ = await this.applicationDbContext.Orders.AddAsync(entity);

            return entity;
        }

        public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var order = await this.applicationDbContext.Orders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (order == default)
                throw new NotFoundEntityException(nameof(Order));

            this.applicationDbContext.Orders.Remove(order);

            return id;
        }

        public async Task UpdateAsync(Order entity, CancellationToken cancellationToken = default)
        {
            var orderUpdate = await this.applicationDbContext.Orders.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

            if (orderUpdate == default)
                throw new NotFoundEntityException(nameof(Order));

            orderUpdate.Status = entity.Status;
            orderUpdate.Address = entity.Address;
            orderUpdate.DeliveryDate = entity.DeliveryDate;
            orderUpdate.Products = entity.Products;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await this.applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}
