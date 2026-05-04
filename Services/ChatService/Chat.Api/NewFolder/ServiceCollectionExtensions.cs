using Chat.Application.Interfaces;
using Chat.Application.Services;
using Chat.Infrastructure.Repositories;

namespace Chat.Api.NewFolder
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddChatServices(this IServiceCollection services)
        {
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IMessageService, MessageService>();

            return services;
        }
    }
}
