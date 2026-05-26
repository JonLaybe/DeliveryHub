using Chat.Application.DTOs;
using Chat.Application.Exceptions;
using Chat.Application.Interfaces;
using Chat.Domain.Entities;
using Chat.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Chat.Application.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _repository;
        private readonly IMessageService _messageService;
        private readonly IOnlineStatusService _onlineStatusService;
        private readonly ICatalogService _catalogService;
        private readonly ILogger<ConversationService> _logger;

        public ConversationService(
            IConversationRepository repository,
            IMessageService messageService,
            IOnlineStatusService onlineStatusService,
            ICatalogService catalogService,
            ILogger<ConversationService> logger)
        {
            _repository = repository;
            _logger = logger;
            _messageService = messageService;
            _onlineStatusService = onlineStatusService;
            _catalogService = catalogService;
        }

        public async Task<Guid> CreateConversationAsync(Guid buyerId, Guid productId)
        {
            var product = await _catalogService.GetProductByIdAsync(productId)
                ?? throw new ProductNotFoundException(productId);

            var sellerId = product.SellerId;
            if (buyerId == sellerId)
            {
                throw new InvalidOperationException("Buyer and seller cannot be the same");
            }

            var existingConversation = await _repository.FindByUsers(buyerId, sellerId);
            if (IsExist(existingConversation))
            {
                return existingConversation!.Id;
            }

            var conversation = GetConversation(buyerId, sellerId);
            await SaveConversationToDb(conversation);

            _logger.LogInformation(
                "Conversation created: Id={Id}, BuyerId={BuyerId}, SellerId={SellerId}, CreatedAt={CreatedAt}",
                conversation.Id,
                conversation.BuyerId,
                conversation.SellerId,
                conversation.CreatedAt);

            return conversation.Id;
        }

        public async Task<IReadOnlyList<ConversationResponse>> GetUserConversationsAsync(Guid userId)
        {
            var conversations = await _repository.GetForUserAsync(userId);
            if (!conversations.Any())
                return [];

            var conversationIds = conversations.Select(c => c.Id).ToList();
            var sellerIds = conversations.Select(c => c.SellerId).Distinct().ToList();

            var statsTask = _messageService.GetConversationStatsAsync(conversationIds, userId);
            var onlineStatusesTask = _onlineStatusService.IsOnlineAsync(sellerIds);
            // сделать получение данных о продавце из сервиса Auth
            await Task.WhenAll(statsTask, onlineStatusesTask);

            var stats = statsTask.Result;
            var onlineStatuses = onlineStatusesTask.Result;

            var conversationDtos = conversations.Select(c =>
            {
                stats.TryGetValue(c.Id, out var stat);

                return new ConversationResponse
                {
                    ConversationId = c.Id,
                    SellerId = c.SellerId,
                    SellerName = $"Магазин {c.SellerId}",
                    UnreadMessagesCount = stat.UnreadCount,
                    LastMessage = stat.LastMessage,
                    LastMessageAt = c.LastMessageAt,
                    IsOnline = onlineStatuses.ContainsKey(c.SellerId) && onlineStatuses[c.SellerId]
                };
            }).ToList();

            return conversationDtos;
        }

        private static bool IsExist(Conversation? conversation)
        {
            if (conversation != null)
            {
                return true;
            }
            return false;
        }

        private static Conversation GetConversation(Guid buyerId, Guid sellerId)
        {
            return new Conversation
            {
                Id = Guid.NewGuid(),
                BuyerId = buyerId,
                SellerId = sellerId,
                Status = ConversationStatus.Open,
                CreatedAt = DateTime.UtcNow
            };
        }

        private async Task SaveConversationToDb(Conversation conversation)
        {
            await _repository.AddAsync(conversation);
            await _repository.SaveChangesAsync();
        }
    }
}
