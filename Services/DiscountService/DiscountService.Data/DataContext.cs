using DiscountService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscountService.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<DiscountUsage> DiscountUsages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Discount
            modelBuilder.Entity<Discount>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Code)
                    .IsUnique();

                entity.HasIndex(e => e.IsActive);

                entity.HasIndex(e => new { e.StartDate, e.EndDate });

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.Value)
                    .HasPrecision(10, 2);

                entity.Property(e => e.MinimumOrderAmount)
                    .HasPrecision(10, 2);

                entity.Property(e => e.MaximumDiscountAmount)
                    .HasPrecision(10, 2);

                // Используем ValueGeneratedOnAdd
                entity.Property(e => e.CreatedAt)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(DateTime.UtcNow);

                entity.Property(e => e.UpdatedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValue(DateTime.UtcNow);

                // Связь с DiscountUsage
                entity.HasMany(d => d.Usages)
                    .WithOne(u => u.Discount)
                    .HasForeignKey(u => u.DiscountId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // DiscountUsage
            modelBuilder.Entity<DiscountUsage>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.DiscountId);

                entity.HasIndex(e => e.OrderId);

                entity.HasIndex(e => e.UserId);

                entity.HasIndex(e => e.UsedAt);

                entity.Property(e => e.AppliedAmount)
                    .HasPrecision(10, 2);

                entity.Property(e => e.OrderTotal)
                    .HasPrecision(10, 2);

                // Используем ValueGeneratedOnAdd
                entity.Property(e => e.UsedAt)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(DateTime.UtcNow);
            });
        }

    }
}
