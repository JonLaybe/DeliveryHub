using Chat.Application.Interfaces;
using Chat.Domain.Entities;
using Chat.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatDbContext _context;

        public MessageRepository(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<List<Message>> GetAllAsync()
        {
            return await _context.Messages
                .AsNoTracking()
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Message>> GetByConversationIdAsync(Guid conversationId)
        {
            return await _context.Messages
                .AsNoTracking()
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountUnreadMessagesAsync(Guid conversationId, Guid userId)
        {
            return await _context.Messages
                .Where(m => m.ConversationId == conversationId
                            && m.SenderId != userId
                            && !m.IsRead)
                .CountAsync();
        }

        public async Task SetMessageIsReadTrueAsync(Guid conversationId, Guid userId)
        {
            var messages = await _context.Messages
                .Where(m => m.ConversationId == conversationId && m.SenderId != userId && !m.IsRead)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
