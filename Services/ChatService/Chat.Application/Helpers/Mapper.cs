using Chat.Application.DTOs;
using Chat.Domain.Entities;

namespace Chat.Application.Helpers
{
    public static class Mapper
    {
        public static List<MessageResponse> MapToMessageResponseList(List<Message> messages)
        {
            return [.. messages.Select(m => new MessageResponse
            {
                MessageId = m.Id,
                SenderId = m.SenderId,
                Text = m.Text ?? string.Empty,
                CreatedAt = m.CreatedAt,
            })];
        }

        public static MessageResponse MapToMessageResponse(Message message)
        {
            return new()
            {
                MessageId = message.Id,
                SenderId = message.SenderId,
                CreatedAt = message.CreatedAt,
                Text = message.Text
            };
        }
    }
}
