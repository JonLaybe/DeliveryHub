using ChatService.DAL.Enums;

namespace ChatService.DAL.Models
{
    public class Message
    {
        public Guid Id { get; set; }

        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        public Guid SenderId { get; set; }
        public SenderRole SenderRole { get; set; }

        public string Text { get; set; }
        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
