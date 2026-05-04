using Chat.Application.DTOs;

namespace Chat.Application.Interfaces
{
    public interface IConversationService
    {
        Task<Guid> CreateConversationAsync(Guid productId, Guid buyerId, Guid sellerId);
        Task<IReadOnlyList<ConversationDto>> GetUserConversationsAsync(Guid userId);
    }
}
