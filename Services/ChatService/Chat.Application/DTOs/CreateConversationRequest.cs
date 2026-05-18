namespace Chat.Application.DTOs
{
    public record CreateConversationRequest(
        string BuyerId,
        string SellerId);
}
