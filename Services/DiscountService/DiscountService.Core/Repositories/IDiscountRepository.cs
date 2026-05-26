using DiscountService.Core.Entities;

namespace DiscountService.Core.Repositories
{
    public interface IDiscountRepository
    {
        Task<Discount?> GetByIdAsync(int id);
        Task<Discount?> GetByCodeAsync(string code);
        Task<IEnumerable<Discount>> GetAllAsync();
        Task<IEnumerable<Discount>> GetActiveDiscountsAsync();
        Task<Discount> AddAsync(Discount discount);
        Task UpdateAsync(Discount discount);
        Task DeleteAsync(int id);
        Task<bool> CodeExistsAsync(string code);
        Task<DiscountUsage> AddUsageAsync(DiscountUsage usage);
        Task<IEnumerable<DiscountUsage>> GetUsagesByDiscountIdAsync(int discountId);
        Task<int> GetUsageCountAsync(int discountId);
        Task<bool> GetUsageAsync(string code, decimal orderAmount, Guid productId);
    }
}
