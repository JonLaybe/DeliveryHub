using Chat.Application.DTOs;
using Chat.Application.Helpers;
using Chat.Application.Interfaces;
using Chat.Domain.Entities;
using Chat.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Chat.Application.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _repository;
        private readonly ILogger<ConversationService> _logger;

        public ConversationService(
            IConversationRepository repository,
            ILogger<ConversationService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Guid> CreateConversationAsync(Guid buyerId, Guid sellerId)
        {
            if (buyerId == sellerId)
            {
                throw new InvalidOperationException("Buyer and seller cannot be the same");
            }

            var existingConversation = await _repository.FindByUsers(buyerId, sellerId);
            if (IsExist(existingConversation))
            {
                throw new InvalidOperationException("Attempt to create new Conversation with existing Buyer-Seller pair");
            }

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                BuyerId = buyerId,
                SellerId = sellerId,
                Status = ConversationStatus.Open,
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow
            };

            await _repository.AddAsync(conversation);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Conversation created: {@conversation}", conversation);

            return conversation.Id;
        }

        public async Task<IReadOnlyList<ConversationDto>> GetUserConversationsAsync(Guid userId)
        {
            var conversations = await _repository.GetForUserAsync(userId);
            var dtos = Mapper.GetConversationDtoList(conversations);
            return dtos;
        }

        private static bool IsExist(Conversation? conversation)
        {
            if (conversation != null)
            {
                return true; 
            }
            return false;
        }
    }
}
