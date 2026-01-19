using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace WebFeedback.Domain
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> optoins) : base(optoins)
        {

        }

        public DbSet<Entities.Review> Reviews { get; set; }

        public DbSet<Entities.Comment> Comments { get; set; } 

        public DbSet<Entities.Like> Likes { get; set; }

        public DbSet<Entities.Dislike> Dislikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Review>()
                .HasMany(r => r.Comments)
                .WithOne(c => c.Review)
                .HasForeignKey(c => c.ReviewId);
            modelBuilder.Entity<Entities.Review>()
                .HasMany(r => r.Likes)
                .WithOne(l => l.Review)
                .HasForeignKey(l => l.ReviewId);
            modelBuilder.Entity<Entities.Review>()
                .HasMany(r => r.Dislikes)
                .WithOne(d => d.Review)
                .HasForeignKey(d => d.ReviewId);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            //{
            //    optionsBuilder.UseSqlite("");
            //}

            base.OnConfiguring(optionsBuilder);
        }
    }
}
