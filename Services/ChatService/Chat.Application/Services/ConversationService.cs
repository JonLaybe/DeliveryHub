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
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<ConversationService> _logger;

        public ConversationService(
            IConversationRepository repository,
            IMessageService messageService,
            IOnlineStatusService onlineStatusService,
            ICatalogService catalogService,
            IUserProfileService userProfileService,
            ILogger<ConversationService> logger)
        {
            _repository = repository;
            _logger = logger;
            _messageService = messageService;
            _onlineStatusService = onlineStatusService;
            _catalogService = catalogService;
            _userProfileService = userProfileService;
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

            List<Guid> userIds = [];

            var isBuyer = conversations.Where(x => x.BuyerId == userId).Any();
            if (isBuyer)
            {
                userIds = [.. conversations.Select(c => c.SellerId).Distinct()];
            }
            else
            {
                userIds = [.. conversations.Select(c => c.BuyerId).Distinct()];
            }

            var statsTask = _messageService.GetConversationStatsAsync(conversationIds, userId);
            var onlineStatusesTask = _onlineStatusService.IsOnlineAsync(userIds);
            var userProfilesTask = _userProfileService.GetUserInfosByIdsAsync(userIds);
            await Task.WhenAll(statsTask, onlineStatusesTask, userProfilesTask);

            var stats = statsTask.Result;
            var onlineStatuses = onlineStatusesTask.Result;
            var userProfiles = userProfilesTask.Result;

            var conversationDtos = conversations.Select(c =>
            {
                stats.TryGetValue(c.Id, out var stat);

                var response = new ConversationResponse();
                if (isBuyer)
                {
                    response.SellerId = c.SellerId;
                    response.SellerName = userProfiles[c.SellerId].SellerName;
                    response.SellerPhoto = userProfiles[c.SellerId].SellerPhoto;
                    response.IsOnline = onlineStatuses.ContainsKey(c.SellerId) && onlineStatuses[c.SellerId];
                }
                else
                {
                    response.SellerId = c.BuyerId;
                    response.SellerName = userProfiles[c.BuyerId].SellerName;
                    response.SellerPhoto = userProfiles[c.BuyerId].SellerPhoto;
                    response.IsOnline = onlineStatuses.ContainsKey(c.BuyerId) && onlineStatuses[c.BuyerId];
                }

                response.ConversationId = c.Id;
                response.UnreadMessagesCount = stat.UnreadCount;
                response.LastMessage = stat.LastMessage;
                response.LastMessageAt = c.LastMessageAt;
                return response;

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
