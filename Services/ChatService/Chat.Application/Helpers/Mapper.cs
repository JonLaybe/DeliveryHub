using Chat.Application.DTOs;
using Chat.Domain.Entities;

namespace Chat.Application.Helpers
{
    public static class Mapper
    {
        public static List<MessageResponse> GetMessageResponseList(List<Message> messages)
        {
            return [.. messages.Select(m => new MessageResponse
            {
                MessageId = m.Id,
                SenderId = m.SenderId,
                Text = m.Text ?? string.Empty,
                CreatedAt = m.CreatedAt,
            })];
        }
    }
}
