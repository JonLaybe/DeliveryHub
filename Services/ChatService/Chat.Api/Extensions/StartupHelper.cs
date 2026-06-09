using Chat.Api.Configs;
using Chat.Api.Hubs;
using Chat.Application;
using Chat.Application.Interfaces;
using Chat.Application.Services;
using Chat.Infrastructure.Persistence;
using Chat.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Security.Claims;

namespace Chat.Api.Extensions
{
    public static class StartupHelper
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddSignalR();
            services.AddSerilog((context, conf) =>
                conf.ReadFrom.Configuration(configuration));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "AuthService",
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),

                    ValidateIssuerSigningKey = true,

                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name,
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var jwksManager = serviceProvider.GetRequiredService<IJwksManager>();

                options.TokenValidationParameters.IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                {
                    return jwksManager.GetKeys(kid);
                };
            });

            services.AddAuthorizationBuilder()
                .AddPolicy("ServiceOnly", p => p.RequireClaim("typ", "service"));

            services.AddChatServices();

            services.Configure<ServicesConfig>(configuration.GetSection("ServicesConfig"));
            services.AddHttpClient<ICatalogService, CatalogService>("CatalogApi", (provider, client) =>
            {
                var config = provider.GetRequiredService<IOptions<ServicesConfig>>().Value;
                client.BaseAddress = new Uri(config.CatalogApi!.BaseUrl);
            });


            var dbConnection = configuration.GetConnectionString("Postgres");
            services.AddDbContext<ChatDbContext>(options =>
                options.UseNpgsql(dbConnection));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "ChatServiceCache";
            });

            services.AddGrpcClient<UserProfileGrpc.UserProfileGrpcClient>(options =>
            {
                var path = configuration.GetSection("GrpcClients:AuthService").Value;
                options.Address = new Uri(path);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ChatService API",
                    Version = "v1",
                    Description = "API для управления диалогами и сообщениями между покупателями и продавцами",
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Введите JWT токен в поле. Пример: eyJhbGciOiJIUzI1NiIs..."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.EnableAnnotations();
            });
        }

        public static async Task ApplyMigrationsAndSeedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

            await context.Database.MigrateAsync();

            if (app.Environment.IsDevelopment())
            {
                await ChatDbSeeder.SeedAsync(context);
            }
        }

        public static void ConfigureMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatService API v1");
                    options.RoutePrefix = "swagger";
                });
            }

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<ChatHub>("/hubs/chat");
            app.MapControllers();
        }
    }
}
