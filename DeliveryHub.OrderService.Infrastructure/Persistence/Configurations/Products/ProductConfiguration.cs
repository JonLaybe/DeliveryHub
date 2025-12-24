using DeliveryHub.OrderService.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryHub.OrderService.Infrastructure.Persistence.Configurations.Products
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            _ = builder.Property(x => x.ArticleNumber).IsRequired();

            _ = builder.Property(x => x.Name).HasMaxLength(120).IsRequired();

            _ = builder.Property(x => x.Price).IsRequired().HasDefaultValue(0);

            _ = builder.Property(x => x.PhotoPreviewUrl).HasMaxLength(250);

            _ = builder.HasOne(p => p.Order)
                .WithMany(ord => ord.Products)
                .HasForeignKey(p => p.OrderId);
        }
    }
}
