using Chat.Domain.Entities;

namespace Chat.Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetMessagesByConversationIdAsync(IEnumerable<Guid> conversationIds);
        Task<List<Message>> GetMessagesByConversationIdAsync(Guid conversationId);
        Task AddAsync(Message message);
        Task SaveChangesAsync();
        Task SetMessageIsReadTrueAsync(Guid conversationId, Guid userId);
    }
}
