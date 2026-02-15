namespace Chat.Application.DTOs
{
    public record CreateConversationRequest(
    Guid ProductId,
    Guid SellerId);
}
