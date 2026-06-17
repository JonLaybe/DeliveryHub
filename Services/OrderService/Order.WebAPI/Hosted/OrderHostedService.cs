using OrderService.Core.Services.Interfaces.Orders;
using Serilog;

namespace OrderService.WebAPI.Hosted
{
    public class OrderHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _runTime = new TimeSpan(10, 30, 0);
        private static readonly TimeZoneInfo _moscowZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

        public OrderHostedService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration)
        {
            this._scopeFactory = scopeFactory;
            this._runTime = configuration.GetValue<TimeSpan>("NotificationSettings:RunTime");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information(messageTemplate: "Запуск отложенной задачи...");

            Task.Run(async () => {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _moscowZone);
                    var nextRun = now.Date.Add(_runTime);
                    if (now > nextRun) nextRun = nextRun.AddDays(1);

                    var delay = nextRun - now;

                    await Task.Delay(delay, cancellationToken);

                    using var scope = _scopeFactory.CreateScope();
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                    await orderService.UpdateStateByDateAsync(cancellationToken);
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information(messageTemplate: "Остановка отложенной задачи...");
            return Task.CompletedTask;
        }
    }
}
