using Catalog.API.Mapper;
using DeliveryHub.Catalog.Domain.Appliaction.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryHub.Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> GetAll([FromServices]CategoryMapper mapper)
        {
            var data = await _categoryRepository.GetAllAsync(default);

            var result = mapper.MapToDtoList(data.ToList());

            return Ok(result);
        }
    }
}
