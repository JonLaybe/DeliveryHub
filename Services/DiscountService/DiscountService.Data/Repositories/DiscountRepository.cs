using DiscountService.Core.Entities;
using DiscountService.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountService.Data.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly DataContext _context;

        public DiscountRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Discount?> GetByIdAsync(int id)
        {
            return await _context.Discounts
                .Include(d => d.Usages)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Discount?> GetByCodeAsync(string code)
        {
            return await _context.Discounts
                .Include(d => d.Usages)
                .FirstOrDefaultAsync(d => d.Code == code);
        }

        public async Task<IEnumerable<Discount>> GetAllAsync()
        {
            return await _context.Discounts
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Discount>> GetActiveDiscountsAsync()
        {
            var now = DateTime.UtcNow;

            return await _context.Discounts
                .Where(d => d.IsActive
                    && d.StartDate <= now
                    && d.EndDate >= now
                    && (d.UsageLimit == null || d.UsageCount < d.UsageLimit))
                .OrderBy(d => d.StartDate)
                .ToListAsync();
        }

        public async Task<Discount> AddAsync(Discount discount)
        {
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
            return discount;
        }

        public async Task UpdateAsync(Discount discount)
        {
            discount.UpdatedAt = DateTime.UtcNow;
            _context.Entry(discount).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount != null)
            {
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _context.Discounts
                .AnyAsync(d => d.Code == code);
        }

        public async Task<DiscountUsage> AddUsageAsync(DiscountUsage usage)
        {
            _context.DiscountUsages.Add(usage);

            // Увеличиваем счетчик использования
            //var discount = await _context.Discounts.FindAsync(usage.DiscountId);
            //if (discount != null)
            //{
            //    discount.UsageCount++;
            //    discount.UpdatedAt = DateTime.UtcNow;
            //}

            await _context.SaveChangesAsync();
            return usage;
        }

        public async Task<IEnumerable<DiscountUsage>> GetUsagesByDiscountIdAsync(int discountId)
        {
            return await _context.DiscountUsages
                .Where(u => u.DiscountId == discountId)
                .OrderByDescending(u => u.UsedAt)
                .ToListAsync();
        }

        public async Task<int> GetUsageCountAsync(int discountId)
        {
            return await _context.DiscountUsages
                .CountAsync(u => u.DiscountId == discountId);
        }

        public async Task<bool> GetUsageAsync(string code, decimal orderAmount, Guid productId)
        {
            var discount = await _context.Discounts
                .Include(d => d.Usages)
                .FirstOrDefaultAsync(u => u.Code == code &&
                    u.Usages.Any(x => x.OrderTotal == orderAmount && x.ProductId == productId));
            return discount != null;
        }
    }
}
