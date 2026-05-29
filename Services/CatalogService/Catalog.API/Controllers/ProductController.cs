using Catalog.API.Contracts;
using Catalog.API.Mapper;
using Catalog.Application.Models;
using Catalog.Application.Services;
using DeliveryHub.Catalog.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Exceptions;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        private readonly IProductSearchService _productSearchService;
        private readonly ProductMapper _productMapper;

        public ProductController(ILogger<ProductController> logger, 
            IProductSearchService productSearchService,
            ProductMapper productMapper,
            IProductService productService)
        {
            _logger = logger;
            _productSearchService = productSearchService;
            _productService = productService;
            _productMapper = productMapper;
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

            var result = _productMapper.Map(data);

            return Ok(result);
        }

        /// <summary>
        /// Поиск товаров по списку id. Например, для получения данных о товарах в корзине или заказе
        /// </summary>
        /// <param name="idList">Список id</param>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<IActionResult> Get([FromQuery]List<Guid> idList)
        {
            var data = await _productService.GetByManyIdAsync(idList, default);

            return Ok(data);
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
        public async Task<IActionResult> Search([FromQuery] ProductSearchQueryRequest searchQueryReq, [FromServices]IValidator<ProductSearchQueryRequest> validator)
        {
            var validation = validator.Validate(searchQueryReq);

            if (!validation.IsValid)
                throw new BadRequestException(validation.Errors.Select(x => x.ErrorMessage).First());

            var searchQuery = _productMapper.Map(searchQueryReq);

            var result = await _productSearchService.SearchAsync(searchQuery, default);

            _logger.LogInformation("Search result: {productCount} products has found by query: {@query}", result.Products.Count, searchQueryReq);

            return Ok(result);
        }
    }
}
