using Chat.Application.Interfaces;
using Chat.Application.Services;
using Chat.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace Chat.Tests
{
    public class MessageServiceTests
    {
        private readonly Mock<IMessageRepository> _messageRepoMock;
        private readonly Mock<IConversationRepository> _conversationRepoMock;
        private readonly Mock<IOnlineStatusService> _onlineStatusServiceMock;
        private readonly Mock<ILogger<MessageService>> _loggerMock;
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly IMessageService _service;

        public MessageServiceTests()
        {
            _messageRepoMock = new Mock<IMessageRepository>();
            _conversationRepoMock = new Mock<IConversationRepository>();
            _onlineStatusServiceMock = new Mock<IOnlineStatusService>();
            _distributedCacheMock = new Mock<IDistributedCache>();
            _loggerMock = new Mock<ILogger<MessageService>>();

            _service = new MessageService(
                _messageRepoMock.Object,
                _conversationRepoMock.Object,
                _onlineStatusServiceMock.Object,
                _loggerMock.Object,
                _distributedCacheMock.Object
            );
        }

        [Fact]
        public async Task SendMessageAsync_Throws_WhenTextIsEmpty()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.SendMessageAsync(Guid.NewGuid(), Guid.NewGuid(), ""));

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.SendMessageAsync(Guid.NewGuid(), Guid.NewGuid(), "   "));
        }

        [Fact]
        public async Task SendMessageAsync_Throws_WhenConversationNotFound()
        {
            var conversationId = Guid.NewGuid();
            _conversationRepoMock.Setup(r => r.GetByIdAsync(conversationId))
                                 .ReturnsAsync((Conversation)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.SendMessageAsync(conversationId, Guid.NewGuid(), "Hello"));
        }

        [Fact]
        public async Task SendMessageAsync_SavesMessage_AndSetsOnline()
        {
            var conversationId = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            var text = "Hello world";

            var conversation = new Conversation
            {
                Id = conversationId,
                LastMessageAt = DateTime.UtcNow.AddMinutes(-5)
            };

            _conversationRepoMock.Setup(r => r.GetByIdAsync(conversationId))
                                 .ReturnsAsync(conversation);

            var result = await _service.SendMessageAsync(conversationId, senderId, text);

            _messageRepoMock.Verify(r => r.AddAsync(It.Is<Message>(m =>
                m.ConversationId == conversationId &&
                m.SenderId == senderId &&
                m.Text == text &&
                m.Id != Guid.Empty &&
                !m.IsRead &&
                m.SenderRole == Domain.Enums.SenderRole.User
            )), Times.Once);

            _messageRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _conversationRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _onlineStatusServiceMock.Verify(s => s.SetOnlineAsync(senderId), Times.Once);

            Assert.True(conversation.LastMessageAt > DateTime.UtcNow.AddSeconds(-1));
            Assert.NotEqual(Guid.Empty, result);
        }



        [Fact]
        public async Task GetMessagesAsync_ReturnsEmptyList_WhenNoMessages()
        {
            var conversationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var cacheKey = $"chat:{conversationId}";

            _distributedCacheMock.Setup(x => x.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            _messageRepoMock.Setup(r => r.GetMessagesByConversationIdAsync(conversationId))
                .ReturnsAsync([]);

            var result = await _service.GetMessagesAsync(conversationId, userId);

            Assert.Empty(result);
            _messageRepoMock.Verify(r => r.SetMessageIsReadTrueAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }



        [Fact]
        public async Task GetConversationStatsAsync_ReturnsCorrectUnreadAndLastMessage()
        {
            var userId = Guid.NewGuid();
            var otherUser1 = Guid.NewGuid();
            var otherUser2 = Guid.NewGuid();
            var conversationId1 = Guid.NewGuid();
            var conversationId2 = Guid.NewGuid();
            var conversationId3 = Guid.NewGuid();

            var now = DateTime.UtcNow;
            var messages = new List<Message>
            {
                CreateMessage(conversationId1, otherUser1, "Msg1", false, now.AddMinutes(-5)),
                CreateMessage(conversationId1, userId, "MyMsg", true, now.AddMinutes(-4)),
                CreateMessage(conversationId1, otherUser2, "Msg3", false, now.AddMinutes(-1)),
                CreateMessage(conversationId2, userId, "MyMsg2", true, now.AddMinutes(-2)),
                CreateMessage(conversationId2, otherUser1, "OldMsg", true, now.AddMinutes(-10)),
            };

            _messageRepoMock.Setup(r => r.GetMessagesByConversationIdAsync(It.IsAny<IEnumerable<Guid>>()))
                            .ReturnsAsync((IEnumerable<Guid> ids) =>
                                messages.Where(m => ids.Contains(m.ConversationId)).ToList());

            var conversationIds = new[] { conversationId1, conversationId2, conversationId3 };

            var result = await _service.GetConversationStatsAsync(conversationIds, userId);

            Assert.Equal(2, result.Count);

            Assert.Contains(conversationId1, result.Keys);
            Assert.Contains(conversationId2, result.Keys);
            Assert.DoesNotContain(conversationId3, result.Keys);

            var (unreadCount1, lastMessage1) = result[conversationId1];
            Assert.Equal(2, unreadCount1);
            Assert.Equal("Msg3", lastMessage1);

            var (unreadCount2, lastMessage2) = result[conversationId2];
            Assert.Equal(0, unreadCount2);
            Assert.Equal("MyMsg2", lastMessage2);
        }

        [Fact]
        public async Task GetConversationStatsAsync_WithEmptyConversationIds_ReturnsEmptyDictionary()
        {
            var conversationIds = new List<Guid>();
            var userId = Guid.NewGuid();

            _messageRepoMock.Setup(r => r.GetMessagesByConversationIdAsync(It.IsAny<IEnumerable<Guid>>()))
                            .ReturnsAsync([]);

            var result = await _service.GetConversationStatsAsync(conversationIds, userId);

            Assert.Empty(result);

            _messageRepoMock.Verify(r => r.GetMessagesByConversationIdAsync(It.IsAny<IEnumerable<Guid>>()), Times.Once);
        }

        [Fact]
        public async Task GetConversationStatsAsync_WithNullConversationIds_ThrowsException()
        {
            IEnumerable<Guid> conversationIds = null;
            var userId = Guid.NewGuid();

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.GetConversationStatsAsync(conversationIds, userId));
        }

        [Fact]
        public async Task SendMessageAsync_UpdatesLastMessageAt_InConversation()
        {
            var conversationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var oldDate = DateTime.UtcNow.AddDays(-1);

            var conversation = new Conversation
            {
                Id = conversationId,
                LastMessageAt = oldDate
            };

            _conversationRepoMock.Setup(r => r.GetByIdAsync(conversationId))
                                 .ReturnsAsync(conversation);

            await _service.SendMessageAsync(conversationId, userId, "Test");

            Assert.True(conversation.LastMessageAt > oldDate);
            Assert.True(conversation.LastMessageAt <= DateTime.UtcNow);
        }

        [Fact]
        public async Task GetMessagesAsync_ReturnsMessages_WhenMessagesExist()
        {
            var conversationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var cacheKey = $"chat:{conversationId}";

            var expectedMessages = new List<Message>
            {
                new() { Id = new Guid("c57e8cfe-0ff2-48f1-aa7c-91fb18267ffe"), ConversationId = conversationId, Text = "Hi", CreatedAt = DateTime.UtcNow.AddMinutes(-2) },
                new() { Id = new Guid("287dce2c-40b7-41b7-b404-1e2e964a1615"), ConversationId = conversationId, Text = "Hello", CreatedAt = DateTime.UtcNow.AddMinutes(-1) }
            };

            _distributedCacheMock.Setup(x => x.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            _messageRepoMock.Setup(r => r.GetMessagesByConversationIdAsync(conversationId))
                .ReturnsAsync(expectedMessages);

            _messageRepoMock.Setup(r => r.SetMessageIsReadTrueAsync(conversationId, userId))
                .Returns(Task.CompletedTask);

            var result = await _service.GetMessagesAsync(conversationId, userId);

            Assert.Equal(2, result.Count);
            Assert.Equal("Hi", result[0].Text);
            Assert.Equal("Hello", result[1].Text);
            Assert.Equal(new Guid("c57e8cfe-0ff2-48f1-aa7c-91fb18267ffe"), result[0].MessageId);
            Assert.Equal(new Guid("287dce2c-40b7-41b7-b404-1e2e964a1615"), result[1].MessageId);

            _messageRepoMock.Verify(r => r.SetMessageIsReadTrueAsync(conversationId, userId), Times.Once);

            _distributedCacheMock.Verify(x => x.SetAsync(
                cacheKey,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetMessagesAsync_ReturnsMessagesInCorrectOrder()
        {
            var conversationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var cacheKey = $"chat:{conversationId}";
            var now = DateTime.UtcNow;

            var messages = new List<Message>
            {
                CreateMessage(conversationId, userId, "First", false, now.AddMinutes(-10)),
                CreateMessage(conversationId, userId, "Second", false, now.AddMinutes(-5)),
                CreateMessage(conversationId, userId, "Third", false, now.AddMinutes(-1))
            };

            _distributedCacheMock.Setup(x => x.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            _messageRepoMock.Setup(r => r.GetMessagesByConversationIdAsync(conversationId))
                .ReturnsAsync(messages);

            _messageRepoMock.Setup(r => r.SetMessageIsReadTrueAsync(conversationId, userId))
                .Returns(Task.CompletedTask);

            var result = await _service.GetMessagesAsync(conversationId, userId);

            Assert.Equal(3, result.Count);
            Assert.Equal("First", result[0].Text);
            Assert.Equal("Second", result[1].Text);
            Assert.Equal("Third", result[2].Text);
        }

        [Fact]
        public async Task GetMessagesAsync_WithoutUserId_ShouldNotMarkAsRead()
        {
            var conversationId = Guid.NewGuid();
            var cacheKey = $"chat:{conversationId}";
            var messages = new List<Message>
            {
                new() { Id = Guid.NewGuid(), ConversationId = conversationId, Text = "Hi" }
            };

            _distributedCacheMock.Setup(x => x.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            _messageRepoMock.Setup(r => r.GetMessagesByConversationIdAsync(conversationId))
                .ReturnsAsync(messages);

            var result = await _service.GetMessagesAsync(conversationId, Guid.Empty);

            Assert.Single(result);
            _messageRepoMock.Verify(r => r.SetMessageIsReadTrueAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
            _distributedCacheMock.Verify(x => x.SetAsync(
                cacheKey,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #region Helper Methods

        private static Message CreateMessage(Guid conversationId, Guid senderId, string text, bool isRead = false, DateTime? createdAt = null)
        {
            return new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                SenderId = senderId,
                Text = text,
                IsRead = isRead,
                CreatedAt = createdAt ?? DateTime.UtcNow,
                SenderRole = Domain.Enums.SenderRole.User
            };
        }

        #endregion
    }
}
