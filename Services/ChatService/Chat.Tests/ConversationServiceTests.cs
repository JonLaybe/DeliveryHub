using Chat.Application.Interfaces;
using Chat.Application.Services;
using Chat.Domain.Entities;
using Chat.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace Chat.Tests
{
    public class ConversationServiceTests
    {
        private readonly Mock<IConversationRepository> _repositoryMock;
        private readonly Mock<IMessageService> _messageServiceMock;
        private readonly Mock<IOnlineStatusService> _onlineStatusServiceMock;
        private readonly Mock<ILogger<ConversationService>> _loggerMock;
        private readonly IConversationService _service;

        public ConversationServiceTests()
        {
            _repositoryMock = new Mock<IConversationRepository>();
            _loggerMock = new Mock<ILogger<ConversationService>>();
            _messageServiceMock = new Mock<IMessageService>();
            _onlineStatusServiceMock = new Mock<IOnlineStatusService>();
            _service = new ConversationService(
                _repositoryMock.Object,
                _messageServiceMock.Object,
                _onlineStatusServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task CreateConversationAsync_ShouldCreateConversation()
        {
            var productId = Guid.NewGuid();
            var buyerId = Guid.NewGuid();
            var sellerId = Guid.NewGuid();

            var id = await _service.CreateConversationAsync(buyerId, sellerId);

            _repositoryMock.Verify(r =>
                r.AddAsync(It.Is<Conversation>(c =>
                    c.BuyerId == buyerId &&
                    c.SellerId == sellerId &&
                    c.Status == ConversationStatus.Open
                )),
                Times.Once);

            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.NotEqual(Guid.Empty, id);
        }

        [Fact]
        public async Task CreateConversationAsync_ShouldThrow_WhenBuyerEqualsSeller()
        {
            var userId = Guid.NewGuid();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateConversationAsync(userId, userId));
        }

        [Fact]
        public async Task CreateConversationAsync_ShouldThrow_WhenBuyerAndSellerExist()
        {
            var conversation = new Conversation()
            {
                Id = Guid.NewGuid(),
                BuyerId = Guid.NewGuid(),
                SellerId = Guid.NewGuid()
            };

            _repositoryMock
                .Setup(r => r.FindByUsers(conversation.BuyerId, conversation.SellerId))
                .ReturnsAsync(conversation);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateConversationAsync(conversation.BuyerId, conversation.SellerId));
        }

        [Fact]
        public async Task GetUserConversationsAsync_ShouldReturnUserConversationsWithStatsAndOnline()
        {
            var userId = Guid.NewGuid();
            var sellerId1 = Guid.NewGuid();
            var sellerId2 = Guid.NewGuid();

            var conversation1 = new Conversation { Id = Guid.NewGuid(), BuyerId = userId, SellerId = sellerId1 };
            var conversation2 = new Conversation { Id = Guid.NewGuid(), BuyerId = Guid.NewGuid(), SellerId = userId };

            var conversations = new List<Conversation> { conversation1, conversation2 };

            _repositoryMock
                .Setup(r => r.GetForUserAsync(userId))
                .ReturnsAsync(conversations);

            _messageServiceMock
                .Setup(s => s.GetConversationStatsAsync(It.IsAny<IEnumerable<Guid>>(), userId))
                .ReturnsAsync((IEnumerable<Guid> ids, Guid uid) =>
                {
                    return ids.ToDictionary(
                        id => id,
                        id => (unreadCount: 5, lastMessage: "Last message " + id.ToString())
                    );
                });

            _onlineStatusServiceMock
                .Setup(s => s.IsOnlineAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync((IEnumerable<Guid> ids) =>
                {
                    return ids.ToDictionary(id => id, id => true);
                });

            var result = await _service.GetUserConversationsAsync(userId);

            Assert.Equal(2, result.Count);

            foreach (var dto in result)
            {
                Assert.Equal(5, dto.UnreadMessagesCount);
                Assert.StartsWith("Last message", dto.LastMessage);
                Assert.True(dto.IsOnline);
                Assert.NotEqual(Guid.Empty, dto.SellerId);
                Assert.StartsWith("Магазин", dto.SellerName);
            }

            _repositoryMock.Verify(r => r.GetForUserAsync(userId), Times.Once);
            _messageServiceMock.Verify(s => s.GetConversationStatsAsync(It.IsAny<IEnumerable<Guid>>(), userId), Times.Once);
            _onlineStatusServiceMock.Verify(s => s.IsOnlineAsync(It.IsAny<IEnumerable<Guid>>()), Times.Once);
        }
    }
}