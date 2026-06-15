using Moq;
using OrderService.Core.Models.Orders;
using OrderService.Core.Models.Products;
using OrderService.Core.Services.Interfaces.Orders;
using OrderService.WebAPI.Controllers.Orders;

namespace OrderService.Test
{
    public class OrderControllerTest
    {
        [TestFixture]
        public class OrderControllerTests
        {
            private Mock<IOrderService> _orderServiceMock;
            private OrderController _controller;

            [SetUp]
            public void Setup()
            {
                _orderServiceMock = new Mock<IOrderService>();
                _controller = new OrderController(_orderServiceMock.Object);
            }

            [Test]
            public async Task GetOrderAsync_ValidId_ReturnsExpectedOrder()
            {
                int orderId = 1;
                var expectedOrder = new OrderDto
                {
                    Id = 1,
                    Address = "Address",
                    CreatedDate = DateTime.Now,
                    DeliveryDate = DateTime.Now,
                    Status = Domain.Enums.Orders.OrderStatus.Relevant,
                };

                var cancellationToken = CancellationToken.None;

                this._orderServiceMock
                    .Setup(s => s.GetEntityAsync(orderId, cancellationToken))
                    .ReturnsAsync(expectedOrder);

                var result = await this._controller.GetOrderAsync(orderId, cancellationToken);

                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.EqualTo(expectedOrder));
                _orderServiceMock.Verify(s => s.GetEntityAsync(orderId, cancellationToken), Times.Once);
            }

            [Test]
            public async Task GetOrdersAsync_WhenCalled_ReturnsListOfOrders()
            {
                var expectedOrders = new List<OrderDto> { new OrderDto(), new OrderDto() };
                var cancellationToken = CancellationToken.None;

                this._orderServiceMock
                    .Setup(s => s.GetOrdersAsync(cancellationToken))
                    .ReturnsAsync(expectedOrders);

                var result = await _controller.GetOrdersAsync(cancellationToken);

                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(expectedOrders.Count));
                this._orderServiceMock.Verify(s => s.GetOrdersAsync(cancellationToken), Times.Once);
            }

            [Test]
            public async Task CreateOrderDto_ValidDto_ReturnsCreatedOrder()
            {
                var createDto = new OrderCreateDto
                {
                    UserId = new Guid(),
                    Address = "Address",
                    DeliveryDate = DateTime.Now,
                    Products = new List<ProductCreateDto>()
                    {
                        new ProductCreateDto()
                        {
                            ArticleNumber = new Guid(),
                            Price = 1050,
                            Quantity = 2,
                        }
                    }
                };
                var expectedOrder = new OrderDto
                {
                    Id = 1,
                    Address = "Address",
                    CreatedDate = DateTime.Now,
                    DeliveryDate = DateTime.Now,
                    Status = Domain.Enums.Orders.OrderStatus.Relevant,
                };
                var cancellationToken = CancellationToken.None;

                this._orderServiceMock
                    .Setup(s => s.AddAsync(createDto, cancellationToken))
                    .ReturnsAsync(expectedOrder);

                var result = await this._controller.CreateOrderDto(createDto, cancellationToken);

                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.EqualTo(expectedOrder));
                this._orderServiceMock.Verify(s => s.AddAsync(createDto, cancellationToken), Times.Once);
            }
        }
    }
}
