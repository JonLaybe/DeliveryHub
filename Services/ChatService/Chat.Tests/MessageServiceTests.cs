using Chat.Application.Interfaces;
using Chat.Application.Services;
using Chat.Domain.Entities;
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
        private readonly MessageService _service;

        public MessageServiceTests()
        {
            _messageRepoMock = new Mock<IMessageRepository>();
            _conversationRepoMock = new Mock<IConversationRepository>();
            _loggerMock = new Mock<ILogger<MessageService>>();
            _onlineStatusServiceMock = new Mock<IOnlineStatusService>();

            _service = new MessageService(
                _messageRepoMock.Object,
                _conversationRepoMock.Object,
                _onlineStatusServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task SendMessageAsync_Throws_WhenTextIsEmpty()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.SendMessageAsync(Guid.NewGuid(), Guid.NewGuid(), ""));
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
            var conversation = new Conversation
            {
                Id = conversationId,
                LastMessageAt = DateTime.UtcNow
            };

            _conversationRepoMock.Setup(r => r.GetByIdAsync(conversationId))
                                 .ReturnsAsync(conversation);

            var senderId = Guid.NewGuid();
            var text = "Hello world";

            var result = await _service.SendMessageAsync(conversationId, senderId, text);

            _messageRepoMock.Verify(r => r.AddAsync(It.Is<Message>(m =>
                m.ConversationId == conversationId &&
                m.SenderId == senderId &&
                m.Text == text
            )), Times.Once);

            _messageRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _conversationRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            _onlineStatusServiceMock.Verify(s => s.SetOnlineAsync(senderId), Times.Once);

            Assert.NotEqual(Guid.Empty, result);
        }

        [Fact]
        public async Task GetMessagesAsync_ReturnsMessages()
        {
            var conversationId = Guid.NewGuid();
            var messages = new List<Message>
            {
                new() { Id = Guid.NewGuid(), ConversationId = conversationId, Text = "Hi" }
            };

            _messageRepoMock.Setup(r => r.GetMessagesByConversationIdAsync(conversationId))
                            .ReturnsAsync(messages);

            var result = await _service.GetMessagesAsync(conversationId);

            Assert.Single(result);
            Assert.Equal("Hi", result[0].Text);
        }

        [Fact]
        public async Task GetConversationStatsAsync_ReturnsCorrectUnreadAndLastMessage()
        {
            var userId = Guid.NewGuid();
            var conversationId1 = Guid.NewGuid();
            var conversationId2 = Guid.NewGuid();

            var messages = new List<Message>
            {
                CreateMessage(conversationId1, Guid.NewGuid(), "Msg1"),
                CreateMessage(conversationId1, userId, "MyMsg", true),
                CreateMessage(conversationId1, Guid.NewGuid(), "Msg2"),
                CreateMessage(conversationId2, userId, "OtherMsg", true),
            };

            _messageRepoMock.Setup(r => r.GetMessagesByConversationIdAsync(It.IsAny<IEnumerable<Guid>>()))
                            .ReturnsAsync((IEnumerable<Guid> ids) => messages.Where(m => ids.Contains(m.ConversationId)).ToList());

            var conversationIds = new[] { conversationId1, conversationId2 };

            var result = await _service.GetConversationStatsAsync(conversationIds, userId);

            Assert.Equal(2, result.Count);

            var (unreadCount, lastMessage) = result[conversationId1];
            Assert.Equal(2, unreadCount);
            Assert.Equal("Msg2", lastMessage);

            var conv2 = result[conversationId2];
            Assert.Equal(0, conv2.unreadCount);
            Assert.Equal("OtherMsg", conv2.lastMessage);
        }

        [Fact]
        public async Task MarkMessagesAsReadAsync_CallsRepository()
        {
            var conversationId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _messageRepoMock.Setup(r => r.SetMessageIsReadTrueAsync(conversationId, userId))
                            .Returns(Task.CompletedTask)
                            .Verifiable();

            await _service.MarkMessagesAsReadAsync(conversationId, userId);

            _messageRepoMock.Verify(r => r.SetMessageIsReadTrueAsync(conversationId, userId), Times.Once);
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
    }
}
