using DiscountService.Abstractions;
using DiscountService.Core.Entities;
using DiscountService.Core.Repositories;
using DiscountService.Core.Services.Abstractions;

namespace DiscountService.Core.Services
{
    public class DiscountProcessor : IDiscountProcessor
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountProcessor(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<Discount?> GetDiscountByIdAsync(int id)
        {
            return await _discountRepository.GetByIdAsync(id);
        }

        public async Task<Discount?> GetDiscountByCodeAsync(string code)
        {
            return await _discountRepository.GetByCodeAsync(code);
        }

        public async Task<IEnumerable<Discount>> GetAllDiscountsAsync()
        {
            return await _discountRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Discount>> GetActiveDiscountsAsync()
        {
            return await _discountRepository.GetActiveDiscountsAsync();
        }

        public async Task<Discount> CreateDiscountAsync(Discount discount)
        {
            // Проверка уникальности кода
            if (await _discountRepository.CodeExistsAsync(discount.Code))
            {
                throw new InvalidOperationException($"Discount code '{discount.Code}' already exists");
            }

            return await _discountRepository.AddAsync(discount);
        }

        public async Task<Discount> UpdateDiscountAsync(int id, Discount discount)
        {
            var existingDiscount = await _discountRepository.GetByIdAsync(id);
            if (existingDiscount == null)
            {
                throw new KeyNotFoundException($"Discount with id {id} not found");
            }

            // Проверка уникальности кода (если изменился)
            if (existingDiscount.Code != discount.Code &&
                await _discountRepository.CodeExistsAsync(discount.Code))
            {
                throw new InvalidOperationException($"Discount code '{discount.Code}' already exists");
            }

            // Обновляем поля
            existingDiscount.Code = discount.Code;
            existingDiscount.Description = discount.Description;
            existingDiscount.DiscountType = discount.DiscountType;
            existingDiscount.Value = discount.Value;
            existingDiscount.StartDate = discount.StartDate;
            existingDiscount.EndDate = discount.EndDate;
            existingDiscount.IsActive = discount.IsActive;
            existingDiscount.UsageLimit = discount.UsageLimit;
            existingDiscount.MinimumOrderAmount = discount.MinimumOrderAmount;
            existingDiscount.MaximumDiscountAmount = discount.MaximumDiscountAmount;

            await _discountRepository.UpdateAsync(existingDiscount);
            return existingDiscount;
        }

        public async Task DeleteDiscountAsync(int id)
        {
            await _discountRepository.DeleteAsync(id);
        }

        public async Task<bool> ValidateDiscountAsync(string code, decimal orderAmount, int userId)
        {
            var discount = await _discountRepository.GetByCodeAsync(code);
            if (discount == null)
                return false;

            var now = DateTime.UtcNow;
            // Проверка активности
            if (!discount.IsActive)
                return false;

            // Проверка дат
            if (now < discount.StartDate || now > discount.EndDate)
                return false;

            // Проверка лимита использования
            if (discount.UsageLimit.HasValue && discount.UsageCount >= discount.UsageLimit.Value)
                return false;

            // Проверка минимальной суммы заказа
            if (discount.MinimumOrderAmount.HasValue && orderAmount < discount.MinimumOrderAmount.Value)
                return false;

            return true;
        }

        public async Task<decimal> CalculateDiscountAsync(string code, decimal orderAmount)
        {
            var discount = await _discountRepository.GetByCodeAsync(code);
            if (discount == null)
                return 0;

            if (!await ValidateDiscountAsync(code, orderAmount, 0))
                return 0;

            decimal discountAmount = 0;

            if (discount.DiscountType == DiscountType.Percentage)
            {
                discountAmount = orderAmount * (discount.Value / 100);
            }
            else if (discount.DiscountType == DiscountType.FixedAmount)
            {
                discountAmount = discount.Value;
            }

            // Проверка максимальной суммы скидки
            if (discount.MaximumDiscountAmount.HasValue &&
                discountAmount > discount.MaximumDiscountAmount.Value)
            {
                discountAmount = discount.MaximumDiscountAmount.Value;
            }

            // Скидка не может быть больше суммы заказа
            if (discountAmount > orderAmount)
            {
                discountAmount = orderAmount;
            }

            return discountAmount;
        }

        public async Task<DiscountUsage> ApplyDiscountAsync(string code, Guid orderId, int userId, decimal orderAmount)
        {
            var discount = await _discountRepository.GetByCodeAsync(code);
            if (discount == null)
                throw new KeyNotFoundException($"Discount code '{code}' not found");

            if (!await ValidateDiscountAsync(code, orderAmount, userId))
                throw new InvalidOperationException($"Discount code '{code}' is not valid for this order");

            var discountAmount = await CalculateDiscountAsync(code, orderAmount);

            var usage = new DiscountUsage
            {
                DiscountId = discount.Id,
                OrderId = orderId,
                UserId = userId,
                AppliedAmount = discountAmount,
                OrderTotal = orderAmount,
                UsedAt = DateTime.UtcNow
            };

            return await _discountRepository.AddUsageAsync(usage);
        }

        public async Task<IEnumerable<DiscountUsage>> GetDiscountUsagesAsync(int discountId)
        {
            return await _discountRepository.GetUsagesByDiscountIdAsync(discountId);
        }
    }
}
