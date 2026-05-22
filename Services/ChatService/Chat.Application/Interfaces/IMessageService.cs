using Chat.Application.DTOs;

namespace Chat.Application.Interfaces
{
    public interface IMessageService
    {
        Task<Guid> SendMessageAsync(Guid conversationId, Guid senderId, string text);
        Task<IReadOnlyList<MessageResponse>> GetMessagesAsync(Guid conversationId);
        Task MarkMessagesAsReadAsync(Guid conversationId, Guid userId);
        Task<Dictionary<Guid, (int unreadCount, string lastMessage)>> GetConversationStatsAsync(IEnumerable<Guid> conversationIds, Guid userId);
    }
}
