using Chat.Application.Interfaces;
using Chat.Application.Services;
using Chat.Infrastructure.Repositories;

namespace Chat.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddChatServices(this IServiceCollection services)
        {
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IOnlineStatusService, OnlineStatusService>();
            services.AddScoped<IUserProfileService, UserProfileService>();

            services.AddSingleton<IJwksManager, JwksManager>();

            return services;
        }
    }
}
