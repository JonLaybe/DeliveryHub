namespace Chat.Application.DTOs
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ConversationId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
