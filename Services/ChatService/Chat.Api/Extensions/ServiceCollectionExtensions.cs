using Chat.Application.DTOs;
using Chat.Application.Interfaces;
using Chat.Application.Services;
using Chat.Application.Validators;
using Chat.Infrastructure.Repositories;
using FluentValidation;

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
            services.AddScoped<IValidator<CreateConversationRequest>, CreateConversationRequestValidator>();

            return services;
        }
    }
}
