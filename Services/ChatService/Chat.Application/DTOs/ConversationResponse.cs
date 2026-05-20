namespace Chat.Application.DTOs
{
    public class ConversationResponse
    {
        public Guid ConversationId { get; set; }
        public Guid SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public int UnreadMessagesCount { get; set; }
    }
}
