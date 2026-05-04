using Chat.Application.DTOs;
using Chat.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task JoinConversation(Guid conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        public async Task SendMessage(Guid conversationId, Guid senderId, string text)
        {
            var messageId = await _messageService.SendMessageAsync(conversationId, senderId, text);

            var message = new MessageDto
            {
                Id = messageId,
                ConversationId = conversationId,
                SenderId = senderId,
                Text = text,
                CreatedAt = DateTime.UtcNow
            };

            await Clients.Group(conversationId.ToString())
                .SendAsync("ReceiveMessage", message);
        }
    }
}
