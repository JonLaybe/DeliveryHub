using DeliveryHub.OrderService.Core.Common.Interfaces;
using DeliveryHub.OrderService.Domain.Entities.Oriders;
using DeliveryHub.OrderService.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DeliveryHub.OrderService.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Order> Orders => this.Set<Order>();

        public DbSet<Product> Products => this.Set<Product>();
    }
}
