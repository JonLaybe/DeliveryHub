using Chat.Application.DTOs;
using Chat.Application.Exceptions;
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
        private readonly Mock<ICatalogService> _catalogServiceMock;
        private readonly Mock<IUserProfileService> _userProfileService;
        private readonly Mock<ILogger<ConversationService>> _loggerMock;

        private readonly IConversationService _service;

        public ConversationServiceTests()
        {
            _repositoryMock = new Mock<IConversationRepository>();
            _loggerMock = new Mock<ILogger<ConversationService>>();
            _messageServiceMock = new Mock<IMessageService>();
            _onlineStatusServiceMock = new Mock<IOnlineStatusService>();
            _catalogServiceMock = new Mock<ICatalogService>();
            _userProfileService = new Mock<IUserProfileService>();

            _service = new ConversationService(
                _repositoryMock.Object,
                _messageServiceMock.Object,
                _onlineStatusServiceMock.Object,
                _catalogServiceMock.Object,
                _userProfileService.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task CreateConversationAsync_ShouldCreateConversation()
        {
            var productId = Guid.NewGuid();
            var buyerId = Guid.NewGuid();
            var sellerId = Guid.NewGuid();

            _catalogServiceMock
                .Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(new ProductDto
                {
                    Id = productId,
                    Description = string.Empty,
                    Name = string.Empty,
                    Price = 0,
                    SellerId = sellerId
                });

            var id = await _service.CreateConversationAsync(buyerId, productId);

            _repositoryMock.Verify(r =>
                r.AddAsync(It.Is<Conversation>(c =>
                    c.BuyerId == buyerId &&
                    c.SellerId == sellerId &&
                    c.Status == ConversationStatus.Open)),
                Times.Once);

            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.NotEqual(Guid.Empty, id);
        }

        [Fact]
        public async Task CreateConversationAsync_ShouldThrow_WhenProductNotFound()
        {
            var buyerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _catalogServiceMock
                .Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync((ProductDto?)null);

            await Assert.ThrowsAsync<ProductNotFoundException>(() =>
                _service.CreateConversationAsync(buyerId, productId));
        }

        [Fact]
        public async Task CreateConversationAsync_ShouldThrow_WhenBuyerEqualsSeller()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _catalogServiceMock
                .Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(new ProductDto
                {
                    Id = productId,
                    Description = string.Empty,
                    Name = string.Empty,
                    Price = 0,
                    SellerId = userId
                });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateConversationAsync(userId, productId));
        }

        [Fact]
        public async Task CreateConversationAsync_ShouldReturnExistingConversationId()
        {
            var buyerId = Guid.NewGuid();
            var sellerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var existingConversation = new Conversation
            {
                Id = Guid.NewGuid(),
                BuyerId = buyerId,
                SellerId = sellerId
            };

            _catalogServiceMock
                .Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(new ProductDto
                {
                    Id = productId,
                    Description = string.Empty,
                    Name = string.Empty,
                    Price = 0,
                    SellerId = sellerId
                });

            _repositoryMock
                .Setup(r => r.FindByUsers(buyerId, sellerId))
                .ReturnsAsync(existingConversation);

            var result = await _service.CreateConversationAsync(buyerId, productId);

            Assert.Equal(existingConversation.Id, result);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Conversation>()), Times.Never);
        }

        [Fact]
        public async Task GetUserConversationsAsync_ShouldReturnEmptyList_WhenUserHasNoConversations()
        {
            var userId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetForUserAsync(userId))
                .ReturnsAsync([]);

            var result = await _service.GetUserConversationsAsync(userId);

            Assert.Empty(result);
            _messageServiceMock.Verify(m => m.GetConversationStatsAsync(It.IsAny<List<Guid>>(), It.IsAny<Guid>()), Times.Never);
            _onlineStatusServiceMock.Verify(o => o.IsOnlineAsync(It.IsAny<List<Guid>>()), Times.Never);
            _userProfileService.Verify(u => u.GetUserInfosByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetUserConversationsAsync_ShouldReturnConversations_WithCorrectData()
        {
            var userId = Guid.NewGuid();
            var sellerId = Guid.NewGuid();
            var conversationId = Guid.NewGuid();
            var lastMessageAt = DateTime.UtcNow;

            var conversations = new List<Conversation>
            {
                new() {
                    Id = conversationId,
                    BuyerId = userId,
                    SellerId = sellerId,
                    Status = ConversationStatus.Open,
                    CreatedAt = DateTime.UtcNow,
                    LastMessageAt = lastMessageAt
                }
            };

            var stats = new Dictionary<Guid, (int UnreadCount, string LastMessage)>
            {
                [conversationId] = (3, "Hello!")
            };

            var onlineStatuses = new Dictionary<Guid, bool>
            {
                [sellerId] = true
            };

            var userProfiles = new Dictionary<Guid, (string SellerName, string SellerPhoto)>
            {
                [sellerId] = ("John Doe", "http://photo.com/john.jpg")
            };

            _repositoryMock
                .Setup(r => r.GetForUserAsync(userId))
                .ReturnsAsync(conversations);

            _messageServiceMock
                .Setup(m => m.GetConversationStatsAsync(
                    It.Is<List<Guid>>(ids => ids.Contains(conversationId)),
                    userId))
                .ReturnsAsync(stats);

            _onlineStatusServiceMock
                .Setup(o => o.IsOnlineAsync(It.Is<List<Guid>>(ids => ids.Contains(sellerId))))
                .ReturnsAsync(onlineStatuses);

            _userProfileService
                .Setup(u => u.GetUserInfosByIdsAsync(
                    It.Is<List<Guid>>(ids => ids.Contains(sellerId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfiles);

            var result = await _service.GetUserConversationsAsync(userId);

            Assert.Single(result);
            var conversation = result.First();
            Assert.Equal(conversationId, conversation.ConversationId);
            Assert.Equal(sellerId, conversation.SellerId);
            Assert.Equal("John Doe", conversation.SellerName);
            Assert.Equal("http://photo.com/john.jpg", conversation.SellerPhoto);
            Assert.Equal(3, conversation.UnreadMessagesCount);
            Assert.Equal("Hello!", conversation.LastMessage);
            Assert.Equal(lastMessageAt, conversation.LastMessageAt);
            Assert.True(conversation.IsOnline);
        }
    }
}