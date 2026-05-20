namespace Chat.Application.DTOs
{
    public class ConversationDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid BuyerId { get; set; }
        public Guid SellerId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastMessageAt { get; set; }
    }
}
