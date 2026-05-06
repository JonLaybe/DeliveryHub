using DiscountService.Abstractions.Discounts.Apply;
using DiscountService.Core.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiscountService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DiscountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Apply")]
        public async Task<ApplyResponseModel> ApplyAsync([FromBody] Apply apply)
        {
            return await _mediator.Send(apply);
        }
    }
}