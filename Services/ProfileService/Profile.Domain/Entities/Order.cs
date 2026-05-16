using Profile.Domain.Enums;

namespace Profile.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime Date { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public ICollection<Item> Items { get; set; } = [];
    }
}
