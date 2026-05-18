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
        private readonly Mock<ILogger<ConversationService>> _loggerMock;
        private readonly IConversationService _service;

        public ConversationServiceTests()
        {
            _repositoryMock = new Mock<IConversationRepository>();
            _loggerMock = new Mock<ILogger<ConversationService>>();
            _service = new ConversationService(_repositoryMock.Object, _loggerMock.Object);
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
        public async Task GetUserConversationsAsync_ShouldReturnUserConversations()
        {
            var userId = Guid.NewGuid();

            var conversations = new List<Conversation>
            {
                new() { Id = Guid.NewGuid(), BuyerId = userId },
                new() { Id = Guid.NewGuid(), SellerId = userId }
            };

            _repositoryMock
                .Setup(r => r.GetForUserAsync(userId))
                .ReturnsAsync(conversations);

            var result = await _service.GetUserConversationsAsync(userId);

            Assert.Equal(2, result.Count);
            _repositoryMock.Verify(r => r.GetForUserAsync(userId), Times.Once);
        }
    }
}