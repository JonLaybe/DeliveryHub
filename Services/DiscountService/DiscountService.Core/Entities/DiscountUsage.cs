using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountService.Core.Entities
{
    public class DiscountUsage
    {
        public int Id { get; set; }
        public int DiscountId { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? UserId { get; set; }
        public decimal AppliedAmount { get; set; }
        public decimal OrderTotal { get; set; }
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        // Навигационное свойство
        public virtual Discount Discount { get; set; } = null!;
    }
}
