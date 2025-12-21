using ChatService.DAL.Enums;

namespace ChatService.DAL.Models
{
    public class Conversation
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        public Guid SellerId { get; set; }
        public Guid BuyerId { get; set; }

        public ConversationStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastMessageAt { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}
