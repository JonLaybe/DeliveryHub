using Microsoft.EntityFrameworkCore;
using Profile.Domain.Entities;

namespace Profile.Infrastructure.Persistence
{
    public class ProfileDbContext : DbContext
    {
        public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Item> Items { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureOrder(modelBuilder);
            ConfigureConversation(modelBuilder);
            ConfigureItem(modelBuilder);
        }

        private static ModelBuilder ConfigureOrder(ModelBuilder modelBuilder)
        {
            return modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.User)
                      .WithMany(r => r.Orders)
                      .HasForeignKey(x => x.UserId);
            });
        }

        private static ModelBuilder ConfigureItem(ModelBuilder modelBuilder)
        {
            return modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name).IsRequired().HasMaxLength(50);

                entity.HasOne(x => x.Order)
                      .WithMany(r => r.Items)
                      .HasForeignKey(x => x.OrderId);
            });
        }

        private static ModelBuilder ConfigureConversation(ModelBuilder modelBuilder)
        {
            return modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(x => x.LastName).IsRequired().HasMaxLength(50);
                entity.Property(x => x.Patronymic).HasMaxLength(50);
                entity.Property(x => x.Email).IsRequired().HasMaxLength(50);
                entity.Property(x => x.Phone).IsRequired();
            });
        }
    }
}
