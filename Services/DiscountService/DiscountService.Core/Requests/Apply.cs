using DiscountService.Abstractions.Discounts.Apply;
using MediatR;

namespace DiscountService.Core.Requests
{
    public class Apply: ApplyModel, IRequest<ApplyResponseModel>
    {
    }
}
