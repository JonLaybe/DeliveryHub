using Catalog.API.Contracts;
using DeliveryHub.Catalog.Domain.Appliaction.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryHub.Catalog.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ILogger<CategoryController> logger, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Получить список всех категорий
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _categoryRepository.GetAllAsync(default);

            // TODO: Маппинг в AutoMapper/Mapperly
            var result = data.Select(c => new CategoryResponseDto(c.Id, c.Name, c.ParentId));

            return Ok(result);
        }
    }
}
