using Chat.Application.Interfaces;
using Chat.Domain.Entities;
using Chat.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Repositories
{
    public class ConversationRepository(ChatDbContext chatDbContext) : IConversationRepository
    {
        public async Task AddAsync(Conversation conversation)
        {
            await chatDbContext.Conversations.AddAsync(conversation);
        }

        public async Task<Conversation?> GetByIdAsync(Guid id)
        {
            return await chatDbContext.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IReadOnlyList<Conversation>> GetForUserAsync(Guid userId)
        {
            return await chatDbContext.Conversations
                .Where(c => c.BuyerId == userId || c.SellerId == userId)
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await chatDbContext.SaveChangesAsync();
        }
    }
}
