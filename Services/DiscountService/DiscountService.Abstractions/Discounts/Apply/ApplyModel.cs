namespace DiscountService.Abstractions.Discounts.Apply
{
    public class ApplyModel
    {
        public string Code { get; set; } = string.Empty;
        public decimal OrderAmount { get; set; }
        public Guid UserId { get; set; }
    }
}
