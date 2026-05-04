using Chat.Application.Interfaces;
using Chat.Application.Services;
using Chat.Domain.Entities;
using Moq;

namespace Chat.Tests
{
    public class MessageServiceTests
    {
        private readonly Mock<IMessageRepository> _messageRepoMock;
        private readonly Mock<IConversationRepository> _conversationRepoMock;
        private readonly Mock<IOnlineStatusService> _onlineStatusServiceMock;
        private readonly MessageService _service;

        public MessageServiceTests()
        {
            _messageRepoMock = new Mock<IMessageRepository>();
            _conversationRepoMock = new Mock<IConversationRepository>();
            _onlineStatusServiceMock = new Mock<IOnlineStatusService>();

            _service = new MessageService(
                _messageRepoMock.Object,
                _conversationRepoMock.Object,
                _onlineStatusServiceMock.Object
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

            _messageRepoMock.Setup(r => r.GetByConversationIdAsync(conversationId))
                            .ReturnsAsync(messages);

            var result = await _service.GetMessagesAsync(conversationId);

            Assert.Single(result);
            Assert.Equal("Hi", result[0].Text);
        }
    }
}
