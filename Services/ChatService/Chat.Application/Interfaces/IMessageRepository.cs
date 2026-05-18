using Chat.Domain.Entities;

namespace Chat.Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetAllAsync();
        Task<List<Message>> GetByConversationIdAsync(Guid conversationId);
        Task AddAsync(Message message);
        Task SaveChangesAsync();
        Task<int> CountUnreadMessagesAsync(Guid conversationId, Guid userId);
        Task SetMessageIsReadTrueAsync(Guid conversationId, Guid userId);
    }
}
