using Chat.Application.Interfaces;
using Chat.Domain.Entities;
using Chat.Domain.Enums;

namespace Chat.Application.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _repository;

        public ConversationService(IConversationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> CreateConversationAsync(Guid productId, Guid buyerId, Guid sellerId)
        {
            if (buyerId == sellerId)
                throw new InvalidOperationException("Buyer and seller cannot be the same");

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                BuyerId = buyerId,
                SellerId = sellerId,
                Status = ConversationStatus.Open,
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow
            };

            await _repository.AddAsync(conversation);
            await _repository.SaveChangesAsync();

            return conversation.Id;
        }

        public async Task<IReadOnlyList<Conversation>> GetUserConversationsAsync(Guid userId)
        {
            return await _repository.GetForUserAsync(userId);
        }
    }
}
