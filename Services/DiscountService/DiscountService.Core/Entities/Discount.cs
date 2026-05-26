using DiscountService.Abstractions;

namespace DiscountService.Core.Entities
{
    public class Discount
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal Value { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int? UsageLimit { get; set; }
        public int UsageCount { get; set; } = 0;
        public decimal? MinimumOrderAmount { get; set; }
        public decimal? MaximumDiscountAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Навигационное свойство
        public virtual ICollection<DiscountUsage> Usages { get; set; } = new List<DiscountUsage>();
    }
}
