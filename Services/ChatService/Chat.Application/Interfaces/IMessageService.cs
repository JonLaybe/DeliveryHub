using Chat.Application.DTOs;

namespace Chat.Application.Interfaces
{
    public interface IMessageService
    {
        Task<Guid> SendMessageAsync(Guid conversationId, Guid userId, string text);
        Task<IReadOnlyList<MessageResponse>> GetMessagesAsync(Guid conversationId, Guid userId);
        Task<Dictionary<Guid, (int UnreadCount, string LastMessage)>> GetConversationStatsAsync(IEnumerable<Guid> conversationIds, Guid userId);
    }
}
