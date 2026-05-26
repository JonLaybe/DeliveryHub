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
            var conversationId = Guid.NewGuid();
            var message = CreateMessage(conversationId, Guid.NewGuid(), "Hello");

            var repo = await GetRepositoryWithMessagesAsync(message);

            var convMessages = await repo.GetMessagesByConversationIdAsync(conversationId);

            Assert.Single(convMessages);
            Assert.Equal("Hello", convMessages.First().Text);
        }

        [Fact]
        public async Task SetMessageIsReadTrueAsync_MarksMessagesAsRead()
        {
            var conversationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var messages = new[]
            {
                CreateMessage(conversationId, otherUserId, "Msg1"),
                CreateMessage(conversationId, otherUserId, "Msg2")
            };

            var repo = await GetRepositoryWithMessagesAsync(messages);

            await repo.SetMessageIsReadTrueAsync(conversationId, userId);
            var updatedMessages = await repo.GetMessagesByConversationIdAsync(conversationId);

            Assert.All(updatedMessages.Where(m => m.SenderId == otherUserId), m => Assert.True(m.IsRead));
        }

        [Fact]
        public async Task GetMessagesByConversationIdAsync_ReturnsCorrectCollection()
        {
            var conversationId1 = Guid.NewGuid();
            var conversationId2 = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var messages = new List<Message>
            {
                CreateMessage(conversationId1, otherUserId, "Msg1"),
                CreateMessage(conversationId1, otherUserId, "Msg2"),
                CreateMessage(conversationId1, userId, "MyMsg"),
                CreateMessage(conversationId1, otherUserId, "Msg3"),
                CreateMessage(conversationId1, otherUserId, "Msg4"),
                CreateMessage(conversationId1, userId, "MyMsg2")
            };

            var repo = await GetRepositoryWithMessagesAsync([.. messages]);

            var actualMessages = await repo.GetMessagesByConversationIdAsync([conversationId1, conversationId2]);

            Assert.Equal(6, actualMessages.Count);
        }

        private static Message CreateMessage(Guid conversationId, Guid senderId, string text, bool isRead = false)
        {
            return new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                SenderId = senderId,
                Text = text,
                IsRead = isRead,
                CreatedAt = DateTime.UtcNow
            };
        }

        private async Task<MessageRepository> GetRepositoryWithMessagesAsync(params Message[] messages)
        {
            var context = GetInMemoryContext();
            var repo = new MessageRepository(context);

            foreach (var message in messages)
            {
                await repo.AddAsync(message);
            }

            await repo.SaveChangesAsync();
            return repo;
        }
    }
}
