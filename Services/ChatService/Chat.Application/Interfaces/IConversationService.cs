using Chat.Application.DTOs;

namespace Chat.Application.Interfaces
{
    public interface IConversationService
    {
        Task<Guid> CreateConversationAsync(Guid userId, Guid productId);
        Task<IReadOnlyList<ConversationResponse>> GetUserConversationsAsync(Guid userId);
    }
}
