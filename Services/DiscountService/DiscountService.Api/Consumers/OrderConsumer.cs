using DiscountService.Core.Repositories;
using MassTransit;
using Shared.Domain.Entities.RabbitMq;

namespace DiscountService.Api.Consumers
{
    public class OrderConsumer : IConsumer<Order>
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly ILogger<OrderConsumer> _logger;

        public OrderConsumer(IDiscountRepository discountRepository, ILogger<OrderConsumer> logger)
        {
            _discountRepository = discountRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Order> context)
        {
            var order = context.Message;
            _logger.LogInformation("Проверка на приход"+context.Message.ToString());

            await _discountRepository.UpdateUsagesByIdAsync(order.DiscountUsageId);
        }
    }
}
