using DiscountService.Abstractions;
using DiscountService.Core.Entities;

namespace DiscountService.Data
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(DataContext context)
        {
            // Проверка наличия записей
            if (!context.Discounts.Any())
            {
                // Добавление начальной записи
                var discount = new Discount
                {
                    Id = 1,
                    Code = "WELCOME10",
                    Description = "10% off on first purchase",
                    DiscountType = DiscountType.Percentage,
                    Value = 10,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(1),
                    IsActive = true,
                    UsageLimit = 100,
                    MinimumOrderAmount = 50,
                    MaximumDiscountAmount = 20,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Discounts.AddAsync(discount);
                await context.SaveChangesAsync();
            }
        }
    }
}
