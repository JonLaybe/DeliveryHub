using Chat.Domain.Entities;
using Chat.Domain.Enums;
using Chat.Infrastructure.Persistence;
using Chat.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chat.Tests
{
    public class ConversationRepositoryTests
    {
        private ChatDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ChatDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddConversation()
        {
            var context = CreateContext();
            var repo = new ConversationRepository(context);

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                BuyerId = Guid.NewGuid(),
                SellerId = Guid.NewGuid(),
                Status = ConversationStatus.Open,
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow
            };

            await repo.AddAsync(conversation);
            await repo.SaveChangesAsync();

            var result = await context.Conversations.FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.Equal(conversation.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnConversationWithMessages()
        {
            var context = CreateContext();
            var repo = new ConversationRepository(context);

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                BuyerId = Guid.NewGuid(),
                SellerId = Guid.NewGuid(),
                Messages =
                [
                    new() 
                    {
                        Id = Guid.NewGuid(),
                        Text = "Hello",
                        CreatedAt = DateTime.UtcNow
                    }
                ]
            };

            context.Conversations.Add(conversation);
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(conversation.Id);

            Assert.NotNull(result);
            Assert.Single(result.Messages);
        }

        [Fact]
        public async Task GetForUserAsync_ShouldReturnUserConversations()
        {
            var context = CreateContext();
            var repo = new ConversationRepository(context);

            var userId = Guid.NewGuid();

            var conversations = new List<Conversation>
            {
                new() 
                {
                    Id = Guid.NewGuid(),
                    BuyerId = userId,
                    LastMessageAt = DateTime.UtcNow
                },
                new() 
                {
                    Id = Guid.NewGuid(),
                    BuyerId = userId,
                    LastMessageAt = DateTime.UtcNow.AddMinutes(-1)
                }
            };

            context.Conversations.AddRange(conversations);
            await context.SaveChangesAsync();

            var result = await repo.GetForUserAsync(userId);

            Assert.Equal(2, result.Count);
            Assert.True(result[0].LastMessageAt >= result[1].LastMessageAt);
        }
    }
}
