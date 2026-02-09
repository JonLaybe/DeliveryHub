using Chat.Domain.Entities;
using Chat.Domain.Enums;
using Chat.Infrastructure.Persistence;

namespace Chat.Infrastructure.Seed
{
    public static class ChatDbSeeder
    {
        public static async Task SeedAsync(ChatDbContext context)
        {
            if (context.Conversations.Any())
                return;

            var conversationId = new Guid("65778a13-1df6-4765-af50-2a1eab0243b8");
            var buyerId = new Guid("c8e4a03b-960e-4874-80b0-fea30a90fc7b");
            var sellerId = new Guid("a642a65b-5e2b-4f70-ac54-73cbc2465273");

            var conversation = new Conversation
            {
                Id = conversationId,
                ProductId = new Guid("49a067a0-b2cb-4839-8c7e-cbe662e009ea"),
                BuyerId = buyerId,
                SellerId = sellerId,
                Status = ConversationStatus.Open,
                CreatedAt = new DateTime(2026, 8, 15, 0, 0, 0, DateTimeKind.Utc),
                LastMessageAt = new DateTime(2026, 8, 16, 0, 0, 0, DateTimeKind.Utc)
            };
            var messages = CreateMessages(conversationId, buyerId, sellerId);

            foreach (var message in messages)
            {
                conversation.Messages.Add(message);
            }

            await context.Conversations.AddAsync(conversation);
            await context.SaveChangesAsync();
        }

        private static List<Message> CreateMessages(Guid conversationId, Guid buyerId, Guid sellerId)
        {
            return
            [
                new()
                {
                    Id = new Guid("0fad1ce3-6c31-46ee-a865-ce98ae148586"),
                    ConversationId = conversationId,
                    SenderId = buyerId,
                    SenderRole = SenderRole.Buyer,
                    Text = "Здравствуйте! Товар ещё доступен?",
                    CreatedAt = new DateTime(2026, 8, 15, 0, 0, 0, DateTimeKind.Utc),
                    IsRead = true
                },
                new()
                {
                    Id = new Guid("4f382330-0f51-42a8-b9ec-ac4ff60592df"),
                    ConversationId = conversationId,
                    SenderId = sellerId,
                    SenderRole = SenderRole.Seller,
                    Text = "Здравствуйте! Да! В наличие еще 5 штук.",
                    CreatedAt = new DateTime(2026, 8, 16, 0, 0, 0, DateTimeKind.Utc),
                    IsRead = false
                }
            ];
        }
    }
}
