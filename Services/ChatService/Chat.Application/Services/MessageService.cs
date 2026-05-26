using Chat.Application.DTOs;
using Chat.Application.Helpers;
using Chat.Application.Interfaces;
using Chat.Domain.Entities;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Chat.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IOnlineStatusService _onlineStatusService;
        private readonly ILogger<MessageService> _logger;
        private readonly IDatabase _dbRedis;

        public MessageService(
            IMessageRepository messageRepository,
            IConversationRepository conversationRepository,
            IOnlineStatusService onlineStatusService,
            ILogger<MessageService> logger,
            IConnectionMultiplexer redis)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _onlineStatusService = onlineStatusService;
            _logger = logger;
            _dbRedis = redis.GetDatabase();
        }

        public async Task<Guid> SendMessageAsync(Guid conversationId, Guid userId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Message text cannot be empty");

            var conversation = await _conversationRepository.GetByIdAsync(conversationId);

            if (conversation == null)
                throw new InvalidOperationException("Conversation not found");

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                SenderId = userId,
                SenderRole = Domain.Enums.SenderRole.User,
                Text = text,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(message);
            await _onlineStatusService.SetOnlineAsync(userId);

            conversation.LastMessageAt = DateTime.UtcNow;

            await _messageRepository.SaveChangesAsync();
            await _conversationRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Message saved. Id={Id}, ConversationId={ConversationId}, SenderId={SenderId}, Text={Text}",
                message.Id,
                message.ConversationId,
                message.SenderId,
                message.Text);

            return message.Id;
        }

        public async Task<IReadOnlyList<MessageResponse>> GetMessagesAsync(Guid conversationId, Guid userId)
        {
            //var cached = GetMessagesFromCachedAsync(conversationId);
            var messages = await _messageRepository.GetMessagesByConversationIdAsync(conversationId);
            await _messageRepository.SetMessageIsReadTrueAsync(conversationId, userId);
            var response = Mapper.GetMessageResponseList(messages);
            return response;
        }

        public async Task<Dictionary<Guid, (int unreadCount, string lastMessage)>> GetConversationStatsAsync(IEnumerable<Guid> conversationIds, Guid userId)
        {
            var messages = await _messageRepository.GetMessagesByConversationIdAsync(conversationIds);

            var grouped = messages
                .GroupBy(m => m.ConversationId)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var unreadCount = g.Count(m => m.SenderId != userId && !m.IsRead);
                        var lastMessage = g.OrderByDescending(m => m.CreatedAt).FirstOrDefault()?.Text ?? string.Empty;
                        return (UnreadCount: unreadCount, LastMessage: lastMessage);
                    });

            return grouped;
        }

        //public async Task SetOnlineAsync(Guid userId)
        //{
        //    var key = GetKey(userId);
        //    await _db.StringSetAsync(key, true, TimeSpan.FromSeconds(OnlineTtlSeconds));
        //}

        //public async Task SetOfflineAsync(Guid userId)
        //{
        //    var key = GetKey(userId);
        //    await _db.KeyDeleteAsync(key);
        //}

        //private async Task<bool> GetMessagesFromCachedAsync(Guid conversationId)
        //{            
        //    return await _dbRedis.StringGetAsync(conversationId.ToString());
        //}
    }
}
