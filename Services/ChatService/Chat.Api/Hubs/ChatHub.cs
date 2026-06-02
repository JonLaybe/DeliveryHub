using Chat.Application.DTOs;
using Chat.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Api.Hubs
{
    [Authorize]
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

        public async Task SendMessage(Guid conversationId, string text)
        {
            var senderId = GetUserIdFromToken();
            if (senderId == Guid.Empty)
            {
                throw new HubException("User not authenticated");
            }

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

        private Guid GetUserIdFromToken()
        {
            var userIdClaim = Context.User?.FindFirst("uid")?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return Guid.Empty;
        }
    }
}
