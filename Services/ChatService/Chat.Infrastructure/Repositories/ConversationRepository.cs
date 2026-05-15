using Chat.Application.Interfaces;
using Chat.Domain.Entities;
using Chat.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly ChatDbContext _chatDbContext;

        public ConversationRepository(ChatDbContext chatDbContext)
        {
            _chatDbContext = chatDbContext;
        }

        public async Task AddAsync(Conversation conversation)
        {
            await _chatDbContext.Conversations.AddAsync(conversation);
        }

        public async Task<Conversation?> GetByIdAsync(Guid id)
        {
            return await _chatDbContext.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IReadOnlyList<Conversation>> GetForUserAsync(Guid userId)
        {
            return await _chatDbContext.Conversations
                .Where(c => c.BuyerId == userId || c.SellerId == userId)
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();
        }

        public async Task<Conversation?> FindByUsers(Guid buyerId, Guid sellerId)
        {
            return await _chatDbContext.Conversations
                .SingleOrDefaultAsync(r => r.BuyerId == buyerId && r.SellerId == sellerId);
        }

        public async Task SaveChangesAsync()
        {
            await _chatDbContext.SaveChangesAsync();
        }
    }
}
