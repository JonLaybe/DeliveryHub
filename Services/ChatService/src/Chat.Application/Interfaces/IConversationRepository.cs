using Chat.Domain.Entities;

namespace Chat.Application.Interfaces
{
    public interface IConversationRepository
    {
        Task<Conversation?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Conversation>> GetForUserAsync(Guid userId);
        Task AddAsync(Conversation conversation);
        Task SaveChangesAsync();
    }
}
