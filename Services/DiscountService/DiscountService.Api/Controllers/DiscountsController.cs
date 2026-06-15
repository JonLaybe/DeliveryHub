using DiscountService.Abstractions.Discounts.Apply;
using DiscountService.Core.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscountService.Api.Controllers
{
    [Authorize]
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
            apply.UserId = GetUserGuid();            
            return await _mediator.Send(apply);
        }
        private Guid GetUserGuid()
        {
            var userId = User.FindFirst("uid")?.Value;
            var currentUser = new Guid(userId!);
            return currentUser;
        }
    }
}