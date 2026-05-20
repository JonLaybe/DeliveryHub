using Chat.Application.DTOs;
using Chat.Application.Helpers;
using Chat.Application.Interfaces;
using Chat.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Chat.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IOnlineStatusService _onlineStatusService;
        private readonly ILogger<MessageService> _logger;

        public MessageService(
            IMessageRepository messageRepository,
            IConversationRepository conversationRepository,
            IOnlineStatusService onlineStatusService,
            ILogger<MessageService> logger)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _onlineStatusService = onlineStatusService;
            _logger = logger;
        }

        public async Task<Guid> SendMessageAsync(Guid conversationId, Guid senderId, string text)
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
                SenderId = senderId,
                Text = text,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(message);
            await _onlineStatusService.SetOnlineAsync(senderId);

            conversation.LastMessageAt = DateTime.UtcNow;

            await _messageRepository.SaveChangesAsync();
            await _conversationRepository.SaveChangesAsync();

            _logger.LogInformation("Message saved: {@message}", message);
            return message.Id;
        }

        public async Task<IReadOnlyList<MessageDto>> GetMessagesAsync(Guid conversationId)
        {
            var messages = await _messageRepository.GetMessagesByConversationIdAsync(conversationId);
            var dtos = Mapper.GetMessageDtoList(messages);
            return dtos;
        }

        public async Task MarkMessagesAsReadAsync(Guid conversationId, Guid userId)
        {
            await _messageRepository.SetMessageIsReadTrueAsync(conversationId, userId);
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
    }
}
