namespace Chat.Application.DTOs
{
    public class MessageResponse
    {
        public Guid MessageId { get; set; }
        public Guid SenderId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
