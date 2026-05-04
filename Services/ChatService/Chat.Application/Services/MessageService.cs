using Chat.Application.DTOs;
using Chat.Application.Helpers;
using Chat.Application.Interfaces;
using Chat.Domain.Entities;

namespace Chat.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IOnlineStatusService _onlineStatusService;

        public MessageService(
            IMessageRepository messageRepository,
            IConversationRepository conversationRepository,
            IOnlineStatusService onlineStatusService)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _onlineStatusService = onlineStatusService;
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

            return message.Id;
        }

        public async Task<IReadOnlyList<MessageDto>> GetMessagesAsync(Guid conversationId)
        {
            var messages = await _messageRepository.GetByConversationIdAsync(conversationId);
            var dtos = Mapper.GetMessageDtoList(messages);
            return dtos;
        }

        public async Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId)
        {
            return await _messageRepository.CountUnreadMessagesAsync(conversationId, userId);
        }

        public async Task MarkMessagesAsReadAsync(Guid conversationId, Guid userId)
        {
            await _messageRepository.SetMessageIsReadTrueAsync(conversationId, userId);
        }
    }
}
