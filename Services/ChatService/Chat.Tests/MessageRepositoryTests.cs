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

        [Fact]
        public async Task CountUnreadMessagesAsync_ReturnsCorrectCount()
        {
            var context = GetInMemoryContext();
            var repo = new MessageRepository(context);

            var conversationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            await repo.AddAsync(new Message { Id = Guid.NewGuid(), ConversationId = conversationId, SenderId = otherUserId, Text = "Msg1", IsRead = false, CreatedAt = DateTime.UtcNow });
            await repo.AddAsync(new Message { Id = Guid.NewGuid(), ConversationId = conversationId, SenderId = otherUserId, Text = "Msg2", IsRead = false, CreatedAt = DateTime.UtcNow });
            await repo.AddAsync(new Message { Id = Guid.NewGuid(), ConversationId = conversationId, SenderId = userId, Text = "MyMsg", IsRead = false, CreatedAt = DateTime.UtcNow });
            await repo.SaveChangesAsync();

            var count = await repo.CountUnreadMessagesAsync(conversationId, userId);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task SetMessageIsReadTrueAsync_MarksMessagesAsRead()
        {
            var context = GetInMemoryContext();
            var repo = new MessageRepository(context);

            var conversationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            await repo.AddAsync(new Message { Id = Guid.NewGuid(), ConversationId = conversationId, SenderId = otherUserId, Text = "Msg1", IsRead = false, CreatedAt = DateTime.UtcNow });
            await repo.AddAsync(new Message { Id = Guid.NewGuid(), ConversationId = conversationId, SenderId = otherUserId, Text = "Msg2", IsRead = false, CreatedAt = DateTime.UtcNow });
            await repo.SaveChangesAsync();

            await repo.SetMessageIsReadTrueAsync(conversationId, userId);

            var messages = await repo.GetByConversationIdAsync(conversationId);
            Assert.All(messages.Where(m => m.SenderId == otherUserId), m => Assert.True(m.IsRead));
        }
    }
}
