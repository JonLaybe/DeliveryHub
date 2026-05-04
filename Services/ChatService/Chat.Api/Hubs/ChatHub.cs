using Microsoft.AspNetCore.SignalR;

namespace Chat.Api.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinConversation(Guid conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        public async Task SendMessage(Guid conversationId, string senderId, string text)
        {
            var message = new
            {
                Id = Guid.NewGuid(),
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
