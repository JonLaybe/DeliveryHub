using Catalog.API.Contracts;
using Catalog.Application.Models;
using Catalog.Application.Services;
using DeliveryHub.Catalog.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        private readonly IProductSearchService _productSearchService;

        public ProductController(ILogger<ProductController> logger, IProductSearchService productSearchService, IProductService productService)
        {
            _logger = logger;
            _productSearchService = productSearchService;
            _productService = productService;
        }

        /// <summary>
        /// Поиск товара по id
        /// </summary>
        /// <param name="id">Id товара</param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var data = await _productService.GetByIdAsync(id, default);

            // TODO: Маппинг в AutoMapper/Mapperly
            var result = new ProductResponseDto(data.Id, data.Name, data.Description, data.Price, data.AvailableQty, data.CategoryId, data.Attributes);

            return Ok(result);
        }

        /// <summary>
        /// Получить список всех продуктов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _productService.GetAllAsync(default);

            return Ok(data);
        }

        /// <summary>
        /// Получить подсказки по названию товара для автодополнения при поиске
        /// </summary>
        /// <param name="query">Строка поиска</param>
        [HttpGet("suggest")]
        public async Task<IActionResult> Suggest([FromQuery] string query)
        {
            var suggestions = await _productSearchService.SuggestAsync(query, default);
            return Ok(suggestions);
        }

        /// <summary>
        /// Поиск товара по названию, описанию, категории и динамическим характеристикам
        /// </summary>
        /// <param name="searchQueryReq">Dto запроса с параметрами поиска</param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery]ProductSearchQueryRequest searchQueryReq)
        {
            var searchQuery = new ProductSearchQuery
            {
                Attributes = searchQueryReq.Attributes,
                CategoryId = searchQueryReq.CategoryId,
                MaxPrice = searchQueryReq.MaxPrice,
                MinPrice = searchQueryReq.MinPrice,
                Page = searchQueryReq.Page,
                PageSize = searchQueryReq.PageSize,
                Sort = searchQueryReq.Sort,
                Text = searchQueryReq.Text,
            };
            var result = await _productSearchService.SearchAsync(searchQuery, default);

            return Ok(result);
        }
    }
}
