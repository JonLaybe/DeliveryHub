using OrderService.Domain.Entities.Oriders;
using OrderService.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Core.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Order> Orders { get; }

        DbSet<Product> Products { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
