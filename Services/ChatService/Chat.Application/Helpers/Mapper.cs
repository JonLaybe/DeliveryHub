using Chat.Application.DTOs;
using Chat.Domain.Entities;

namespace Chat.Application.Helpers
{
    public static class Mapper
    {
        public static List<MessageDto> GetMessageDtoList(List<Message> messages)
        {
            return [.. messages.Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                ConversationId = m.ConversationId,
                Text = m.Text ?? string.Empty,
                CreatedAt = m.CreatedAt
            })];
        }
    }
}
