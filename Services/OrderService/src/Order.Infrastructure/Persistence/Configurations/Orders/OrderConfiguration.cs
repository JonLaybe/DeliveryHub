using OrderService.Domain.Entities.Oriders;
using OrderService.Domain.Enums.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderService.Infrastructure.Persistence.Configurations.Orders
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            _ = builder.Property(x => x.OrderNumber)
                .IsRequired();

            _ = builder.Property(x => x.Quantity).IsRequired().HasDefaultValue(0);

            _ = builder.Property(x => x.Status).IsRequired().HasDefaultValue(OrderStatus.Unknown);

            _ = builder.Property(x => x.Address).HasMaxLength(150).IsRequired();

            _ = builder.Property(x => x.CreatedDate).IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValue(DateTime.UtcNow);

            _ = builder.HasMany(x => x.Products)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId);
        }
    }
}
