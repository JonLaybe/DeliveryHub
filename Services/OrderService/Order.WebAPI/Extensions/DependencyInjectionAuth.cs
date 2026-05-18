using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace OrderService.WebAPI.Extensions
{
    public static class DependencyInjectionAuth
    {
        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "AuthService",
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.FromSeconds(30),

                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,

                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name
                };

                options.TokenValidationParameters.IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    var clientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                    var client = clientFactory.CreateClient();
                    var jwksUrl = configuration["JWTSettings:jwksUrl"];

                    try
                    {
                        var response = client.GetStringAsync(jwksUrl).GetAwaiter().GetResult();
                        var jwks = new JsonWebKeySet(response);

                        Console.WriteLine($"Information: get jwks.");

                        return jwks.GetSigningKeys();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error get jwks: {ex.Message}");
                        return new List<SecurityKey>();
                    }
                };
            });

            services.AddAuthorization(o =>
            {
                o.AddPolicy("UserOnly", p => p.RequireClaim("typ", "user"));
            });

            return services;
        }
    }
}
