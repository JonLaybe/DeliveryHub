using DiscountService.Core.Entities;
using DiscountService.Core.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DiscountService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : ControllerBase
    {
        private readonly IDiscountProcessor _discountService;

        public DiscountsController(IDiscountProcessor discountService)
        {
            _discountService = discountService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Discount>>> GetAll()
        {
            var discounts = await _discountService.GetAllDiscountsAsync();
            return Ok(discounts);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Discount>>> GetActive()
        {
            var discounts = await _discountService.GetActiveDiscountsAsync();
            return Ok(discounts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Discount>> GetById(int id)
        {
            var discount = await _discountService.GetDiscountByIdAsync(id);
            if (discount == null)
                return NotFound();

            return Ok(discount);
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<Discount>> GetByCode(string code)
        {
            var discount = await _discountService.GetDiscountByCodeAsync(code);
            if (discount == null)
                return NotFound();

            return Ok(discount);
        }

        [HttpPost]
        public async Task<ActionResult<Discount>> Create(Discount discount)
        {
            try
            {
                var createdDiscount = await _discountService.CreateDiscountAsync(discount);
                return CreatedAtAction(nameof(GetById), new { id = createdDiscount.Id }, createdDiscount);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Discount>> Update(int id, Discount discount)
        {
            try
            {
                var updatedDiscount = await _discountService.UpdateDiscountAsync(id, discount);
                return Ok(updatedDiscount);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _discountService.DeleteDiscountAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}/usages")]
        public async Task<ActionResult<IEnumerable<DiscountUsage>>> GetUsages(int id)
        {
            var usages = await _discountService.GetDiscountUsagesAsync(id);
            return Ok(usages);
        }

        [HttpPost("validate")]
        public async Task<ActionResult<bool>> Validate([FromBody] ValidateDiscountRequest request)
        {
            var isValid = await _discountService.ValidateDiscountAsync(
                request.Code,
                request.OrderAmount,
                request.UserId);

            return Ok(isValid);
        }

        [HttpPost("calculate")]
        public async Task<ActionResult<decimal>> Calculate([FromBody] CalculateDiscountRequest request)
        {
            var discountAmount = await _discountService.CalculateDiscountAsync(
                request.Code,
                request.OrderAmount);

            return Ok(discountAmount);
        }

        [HttpPost("apply")]
        public async Task<ActionResult<DiscountUsage>> Apply([FromBody] ApplyDiscountRequest request)
        {
            try
            {
                var usage = await _discountService.ApplyDiscountAsync(
                    request.Code,
                    request.OrderId,
                    request.UserId,
                    request.OrderAmount);

                return Ok(usage);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class ValidateDiscountRequest
    {
        public string Code { get; set; } = string.Empty;
        public decimal OrderAmount { get; set; }
        public int UserId { get; set; }
    }

    public class CalculateDiscountRequest
    {
        public string Code { get; set; } = string.Empty;
        public decimal OrderAmount { get; set; }
    }

    public class ApplyDiscountRequest
    {
        public string Code { get; set; } = string.Empty;
        public Guid OrderId { get; set; }
        public int UserId { get; set; }
        public decimal OrderAmount { get; set; }
    }
}