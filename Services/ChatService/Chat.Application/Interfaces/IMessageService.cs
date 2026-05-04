using Chat.Application.DTOs;

namespace Chat.Application.Interfaces
{
    public interface IMessageService
    {
        Task<Guid> SendMessageAsync(Guid conversationId, Guid senderId, string text);
        Task<IReadOnlyList<MessageDto>> GetMessagesAsync(Guid conversationId);
    }
}
