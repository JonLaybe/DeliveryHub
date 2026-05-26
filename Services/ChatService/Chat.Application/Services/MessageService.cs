using Chat.Application.DTOs;
using Chat.Application.Helpers;
using Chat.Application.Interfaces;
using Chat.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Chat.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IOnlineStatusService _onlineStatusService;
        private readonly ILogger<MessageService> _logger;
        private readonly IDistributedCache _distributedCache;

        private const string Key = "chat:";
        private const int MessageTtlSeconds = 10;

        public MessageService(
            IMessageRepository messageRepository,
            IConversationRepository conversationRepository,
            IOnlineStatusService onlineStatusService,
            ILogger<MessageService> logger,
            IDistributedCache distributedCache)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _onlineStatusService = onlineStatusService;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<Guid> SendMessageAsync(Guid conversationId, Guid userId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Message text cannot be empty");

            var conversation = await _conversationRepository.GetByIdAsync(conversationId);

            if (conversation == null)
                throw new InvalidOperationException("Conversation not found");

            var message = GetMessage(conversationId, userId, text);

            await _messageRepository.AddAsync(message);
            await _messageRepository.SaveChangesAsync();

            conversation.LastMessageAt = DateTime.UtcNow;
            await _conversationRepository.SaveChangesAsync();

            await _onlineStatusService.SetOnlineAsync(userId);

            await UpdateCacheWithNewMessageAsync(conversationId, message);

            _logger.LogInformation(
                "Message sent successfully. MessageId: {MessageId}, ConversationId: {ConversationId}, " +
                "SenderId: {SenderId}, Text length: {TextLength}",
                message.Id, conversationId, userId, text.Length);

            return message.Id;
        }

        public async Task<IReadOnlyList<MessageResponse>> GetMessagesAsync(Guid conversationId, Guid userId)
        {

            var cachedMessages = await GetMessagesFromCacheAsync(conversationId);

            if (cachedMessages.Count != 0)
            {
                _logger.LogDebug("Cache hit for conversation {ConversationId}. Retrieved {Count} messages",
                    conversationId, cachedMessages.Count);
                return cachedMessages;
            }

            _logger.LogDebug("Cache miss for conversation {ConversationId}. Loading from database", conversationId);

            var messages = await _messageRepository.GetMessagesByConversationIdAsync(conversationId);

            if (messages.Count == 0)
            {
                _logger.LogInformation("No messages found for conversation {ConversationId}", conversationId);
                return [];
            }

            await _messageRepository.SetMessageIsReadTrueAsync(conversationId, userId);

            var messageResponses = Mapper.MapToMessageResponseList(messages);
            await SaveMessagesToCacheAsync(conversationId, messageResponses);

            _logger.LogInformation(
                "Loaded {Count} messages from database for conversation {ConversationId}. " +
                "Message IDs: {MessageIds}",
                messageResponses.Count, conversationId,
                string.Join(", ", messageResponses.Take(5).Select(m => m.MessageId)));

            return messageResponses;
        }

        public async Task<Dictionary<Guid, (int UnreadCount, string LastMessage)>> GetConversationStatsAsync(IEnumerable<Guid> conversationIds, Guid userId)
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

        #region Private Methods
        private async Task<List<MessageResponse>> GetMessagesFromCacheAsync(Guid conversationId)
        {
            var key = GetKey(conversationId);
            var stringFromCache = await _distributedCache.GetStringAsync(key);
            if (stringFromCache != null)
            {
                var messageResponseList = JsonSerializer.Deserialize<List<MessageResponse>>(stringFromCache);
                return messageResponseList!;
            }
            return [];
        }

        private async Task SaveMessagesToCacheAsync(Guid conversationId, List<MessageResponse> messages)
        {
            var messagesString = JsonSerializer.Serialize(messages);
            var key = GetKey(conversationId);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(MessageTtlSeconds)
            };
            await _distributedCache.SetStringAsync(key, messagesString, options);
        }

        private static string GetKey(Guid conversationId) => $"{Key}{conversationId}";

        private static Message GetMessage(Guid conversationId, Guid userId, string text)
        {
            return new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                SenderId = userId,
                SenderRole = Domain.Enums.SenderRole.User,
                Text = text,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        private async Task UpdateCacheWithNewMessageAsync(Guid conversationId, Message newMessage)
        {
            try
            {
                var cachedMessages = await GetMessagesFromCacheAsync(conversationId);
                var newMessageResponse = Mapper.MapToMessageResponse(newMessage);

                cachedMessages.Add(newMessageResponse);

                await SaveMessagesToCacheAsync(conversationId, cachedMessages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update cache with new message for conversation {ConversationId}", conversationId);
            }
        }
        #endregion
    }
}
