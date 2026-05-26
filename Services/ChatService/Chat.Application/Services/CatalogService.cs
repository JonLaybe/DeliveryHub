using Chat.Application.DTOs;
using Chat.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;

namespace Chat.Application.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ConversationService> _logger;

        private const string Endpoint = "/api/Product";

        public CatalogService(
            HttpClient httpClient,
            ILogger<ConversationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid productId)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"{Endpoint}/{productId}");
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                response.EnsureSuccessStatusCode();
                var productDto = await response.Content.ReadFromJsonAsync<ProductDto>();
                productDto!.SellerId = Guid.NewGuid(); // заглушка
                return productDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обращении к CatalogService");
                throw;
            }
        }
    }
}
