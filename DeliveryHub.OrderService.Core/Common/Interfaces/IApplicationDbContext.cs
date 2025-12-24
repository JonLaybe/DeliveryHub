using DeliveryHub.OrderService.Domain.Entities.Oriders;
using DeliveryHub.OrderService.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;

namespace DeliveryHub.OrderService.Core.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Order> Orders { get; }

        DbSet<Product> Products { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
