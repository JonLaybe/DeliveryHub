using Microsoft.EntityFrameworkCore;
using OrderService.Core.Common.Interfaces;
using OrderService.Core.Repositories.Interfaces;
using OrderService.Core.Repositories.Interfaces.Orders;
using OrderService.Domain.Entities.Oriders;
using Shared.Abstraction.Exceptions;

namespace OrderService.Core.Repositories.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private IApplicationDbContext applicationDbContext;

        public OrderRepository(IApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<Order> CreateAsync(Order entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            //var order = new Order()
            //{
            //    Address = entity.Address,
            //    Status = entity.Status,
            //    Quantity = entity.Quantity,
            //    DeliveryDate = entity.DeliveryDate,
            //    Products = entity.Products,
            //};

            _ = await this.applicationDbContext.Orders.AddAsync(entity);

            _ = await this.applicationDbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var order = await this.applicationDbContext.Orders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (order == default)
                throw new NotFoundEntityException(nameof(Order));

            this.applicationDbContext.Orders.Remove(order);

            _ = await this.applicationDbContext.SaveChangesAsync(cancellationToken);

            return id;
        }

        public Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Order> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var order = await this.applicationDbContext.Orders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (order == default)
                throw new NotFoundEntityException(nameof(Order));

            return order;
        }

        public async Task UpdateAsync(Order entity, CancellationToken cancellationToken = default)
        {
            var orderUpdate = await this.applicationDbContext.Orders.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

            if (orderUpdate == default)
                throw new NotFoundEntityException(nameof(Order));

            orderUpdate.Quantity = entity.Quantity;
            orderUpdate.Status = entity.Status;
            orderUpdate.Address = entity.Address;
            orderUpdate.DeliveryDate = entity.DeliveryDate;
            orderUpdate.Products = entity.Products;

            _ = await this.applicationDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
