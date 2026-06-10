using Catalog.Application.Repositories;
using MassTransit;
using MongoDB.Driver.Linq;
using Shared.Domain.Entities.RabbitMq;

namespace Catalog.Infrastructure.Messaging.Consumers
{
    public class OrderCreatedMessageConsumer : IConsumer<Shared.Domain.Entities.RabbitMq.Order>
    {
        private readonly IStockRepository _stockRepository;

        public OrderCreatedMessageConsumer(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task Consume(ConsumeContext<Order> context)
        {
            var order = context.Message;

            var productsOnStock = await _stockRepository.GetAll()
                .Where(x => order.Products.Select(prd => prd.Id).Contains(x.ProductId))
                .ToListAsync();

            foreach (var stock in productsOnStock)
            {
                var orderedProd = order.Products.First(x => x.Id == stock.ProductId);
                stock.TotalQty -= orderedProd.Quantity;

                await _stockRepository.UpdateAsync(stock, default);
            }
        }
    }
}
