using DiscountService.Core.Entities;

namespace DiscountService.Core.Services.Abstractions
{
    public interface IDiscountProcessor
    {
        Task<Discount?> GetDiscountByIdAsync(int id);
        Task<Discount?> GetDiscountByCodeAsync(string code);
        Task<IEnumerable<Discount>> GetAllDiscountsAsync();
        Task<IEnumerable<Discount>> GetActiveDiscountsAsync();
        Task<Discount> CreateDiscountAsync(Discount discount);
        Task<Discount> UpdateDiscountAsync(int id, Discount discount);
        Task DeleteDiscountAsync(int id);
        Task<bool> ValidateDiscountAsync(string code, decimal orderAmount, int userId);
        Task<decimal> CalculateDiscountAsync(string code, decimal orderAmount);
        Task<DiscountUsage> ApplyDiscountAsync(string code, Guid orderId, int userId, decimal orderAmount);
        Task<IEnumerable<DiscountUsage>> GetDiscountUsagesAsync(int discountId);
    }
}