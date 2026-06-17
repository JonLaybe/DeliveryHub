using DiscountService.Abstractions;
using DiscountService.Core.Entities;

namespace DiscountService.Data
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(DataContext context)
        {
            if (!context.Discounts.Any())
            {
                var discounts = new List<Discount>
                {
                    new Discount
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
                        MinimumOrderAmount = 500,
                        MaximumDiscountAmount = 2000,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Discount
                    {
                        Id = 2,
                        Code = "SUMMER20",
                        Description = "20% off during summer sales",
                        DiscountType = DiscountType.Percentage,
                        Value = 20,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(3),
                        IsActive = true,
                        UsageLimit = 500,
                        MinimumOrderAmount = 1000,
                        MaximumDiscountAmount = 1000,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Discount
                    {
                        Id = 3,
                        Code = "FIXED500",
                        Description = "$500 off on orders over $2000",
                        DiscountType = DiscountType.FixedAmount,
                        Value = 500,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddDays(14),
                        IsActive = true,
                        UsageLimit = 500,
                        MinimumOrderAmount = 2000,
                        MaximumDiscountAmount = null, // Нет ограничения
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.Discounts.AddRange(discounts);
                await context.SaveChangesAsync();
            }
        }
    }
}
