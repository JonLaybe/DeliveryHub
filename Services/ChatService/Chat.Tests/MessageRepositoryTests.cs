using Chat.Domain.Entities;
using Chat.Infrastructure.Persistence;
using Chat.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chat.Tests
{
    public class MessageRepositoryTests
    {
        private ChatDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ChatDbContext(options);
        }

        [Fact]
        public async Task AddAndGetMessages_WorksCorrectly()
        {
            var context = GetInMemoryContext();
            var repo = new MessageRepository(context);

            var conversationId = Guid.NewGuid();
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                SenderId = Guid.NewGuid(),
                Text = "Hello",
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(message);
            await repo.SaveChangesAsync();

            var allMessages = await repo.GetAllAsync();
            var convMessages = await repo.GetByConversationIdAsync(conversationId);

            Assert.Single(allMessages);
            Assert.Single(convMessages);
            Assert.Equal("Hello", convMessages.First().Text);
        }
    }
}
