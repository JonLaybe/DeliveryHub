using Chat.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Persistence
{
    public class ChatDbContext : DbContext
    {
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<Message> Messages => Set<Message>();

        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureMessage(modelBuilder);
            ConfigureConversation(modelBuilder);
        }

        private static ModelBuilder ConfigureMessage(ModelBuilder modelBuilder)
        {
            return modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Text).HasMaxLength(5000);

                entity.HasOne(x => x.Conversation)
                      .WithMany(r => r.Messages)
                      .HasForeignKey(x => x.ConversationId);

            });
        }

        private static ModelBuilder ConfigureConversation(ModelBuilder modelBuilder)
        {
            return modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Status).IsRequired();
                entity.Property(x => x.CreatedAt).IsRequired();
            });
        }
    }
}
