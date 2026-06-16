using DiscountService.Core.Repositories;
using MassTransit;

namespace DiscountService.Api.Consumers
{
    public class OrderCreatedMessageConsumer : IConsumer<Order>
    {
        private readonly IDiscountRepository _discountRepository;

        public OrderCreatedMessageConsumer(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task Consume(ConsumeContext<Order> context)
        {
            var order = context.Message;

            await _discountRepository.UpdateUsagesByIdAsync(order.DiscountUsageId);
        }
    }
}
