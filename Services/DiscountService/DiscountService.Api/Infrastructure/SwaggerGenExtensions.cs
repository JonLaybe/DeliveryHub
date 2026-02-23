using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace DiscountService.Api.Infrastructure
{
    public static class SwaggerGenExtensions
    {
        public static void ConfigureSwagger(this SwaggerGenOptions options, string serviceName)
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3),
                    Title = serviceName,
                    Description = serviceName,
                    TermsOfService = null,
                    License = new OpenApiLicense { Name = "Proprietary" }
                });

            options.EnableAnnotations();

            var security = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                new List<string>()
            }
        };

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http
            });
            options.AddSecurityRequirement(security);

            // Use FullNames as SchemaIds to prevent name duplication crash
            options.CustomSchemaIds(type => type.FullName);
            options.SchemaFilter<SwaggerGenDuplicateSchemaFilter>();

            // Xml Documentation for Release configuration
            var includeXmlFiles = new[]
            {
            $"{serviceName}.Abstractions.xml",
            $"{serviceName}.Core.xml",
            $"{serviceName}.Web.xml"
        };

            foreach (var xmlFile in includeXmlFiles)
            {
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            }
        }
    }
}
