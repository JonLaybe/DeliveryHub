using Chat.Domain.Entities;

namespace Chat.Application.Interfaces
{
    public interface IConversationService
    {
        Task<Guid> CreateConversationAsync(Guid productId, Guid buyerId, Guid sellerId);
        Task<IReadOnlyList<Conversation>> GetUserConversationsAsync(Guid userId);
    }
}
