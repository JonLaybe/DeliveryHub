using Chat.Api.NewFolder;

namespace Chat.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            StartupHelper.ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            await StartupHelper.ApplyMigrationsAndSeedAsync(app);

            StartupHelper.ConfigureMiddleware(app);

            app.Run();
        }
    }
}