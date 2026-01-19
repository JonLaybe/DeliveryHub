using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebFeedback.Abstractions;
using WebFeedback.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebFeedback.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IRepository<Review> _repository;

        public ReviewController(IRepository<Review> reviewRepository)
        {
            _repository = reviewRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Review>> Get()
        {
            return await _repository.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<Review?> Get(int id)
        {
            return await _repository.GetAsync(id);
        }

        [HttpPost]
        public async Task<Review> Post([FromBody] Review entity)
        {
            await _repository.AddAsync(entity);
            return entity;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Review entity)
        {
            var company = await _repository.GetAsync(id);
            if (company == null)
            {
                return BadRequest("Комания не существует");
            }

            await _repository.UpdateAsync(entity);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var company = await _repository.GetAsync(id);
            if (company == null)
            {
                return BadRequest("Компания не существует");
            }

            await _repository.DeleteAsync(id);
            return NoContent();
        }

    }
}
