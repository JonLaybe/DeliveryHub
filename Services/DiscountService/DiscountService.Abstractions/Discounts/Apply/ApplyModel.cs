namespace DiscountService.Abstractions.Discounts.Apply
{
    public class ApplyModel
    {
        public string Code { get; set; } = string.Empty;
        public Guid OrderId { get; set; }
        public int UserId { get; set; }
        public decimal OrderAmount { get; set; }
    }
}
