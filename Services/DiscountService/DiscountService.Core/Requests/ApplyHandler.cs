using DiscountService.Abstractions.Discounts.Apply;
using DiscountService.Core.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace DiscountService.Core.Requests
{
    public class ApplyHandler : BaseRequestHandler<Apply, ApplyResponseModel>
    {
        private readonly IDiscountProcessor _discountService;
        private readonly ILogger<ApplyHandler> _logger;
        public ApplyHandler(IDiscountProcessor discountProcessor, ILogger<ApplyHandler> logger)
        {
            _discountService = discountProcessor;
            _logger = logger;
        }
        public override async Task<ApplyResponseModel> HandleAsync(Apply request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _discountService.ApplyDiscountAsync(request.Code, request.OrderId, request.UserId, request.OrderAmount);
                var discount = await _discountService.GetDiscountByCodeAsync(request.Code);
                if (discount == null)
                {
                    throw new KeyNotFoundException($"Discount code '{request.Code}' not found");
                }
                return new ApplyResponseModel() { Success = true, AppliedAmount = result.AppliedAmount, DiscountType = discount.DiscountType, Code = request.Code };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ApplyResponseModel() { Success = false, Message = "Промокод не действителен", Code = request.Code };
            }
        }
    }
}
