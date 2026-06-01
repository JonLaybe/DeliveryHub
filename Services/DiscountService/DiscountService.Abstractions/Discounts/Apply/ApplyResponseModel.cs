namespace DiscountService.Abstractions.Discounts.Apply
{
    public class ApplyResponseModel
    {
        public bool Success {  get; set; }
        public string Code { get; set; }
        public decimal AppliedAmount { get; set; }
        public DiscountType DiscountType { get; set; }
        public string Message { get; set; }
    }
}