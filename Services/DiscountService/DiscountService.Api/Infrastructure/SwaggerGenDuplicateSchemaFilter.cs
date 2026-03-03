using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace DiscountService.Api.Infrastructure
{
    public class SwaggerGenDuplicateSchemaFilter : ISchemaFilter
    {
        private readonly HashSet<string> _knownSchemas;
        public SwaggerGenDuplicateSchemaFilter()
        {
            _knownSchemas = new HashSet<string>();
        }

        private static string FormatGenericTypes(Type type)
        {
            if (type.GenericTypeArguments.Length == 0)
            {
                return "";
            }

            var sb = new StringBuilder("<");
            foreach (var innerGeneric in type.GenericTypeArguments)
            {
                sb.Append(innerGeneric.Name);
                sb.Append(FormatGenericTypes(innerGeneric));
            }
            sb.Append('>');
            return sb.ToString();
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            // Make it only works with object types (and enums), so we wont see title param in other generic types
            if (schema.Type != "object" && !context.Type.IsEnum)
            {
                return;
            }

            var sb = new StringBuilder(context.Type.Name);

            // Add Generic Types into Title (Such as ApiResults)
            sb.Append(FormatGenericTypes(context.Type));

            // If Duplicate Found, display Fullname in square brackets near Typename
            if (_knownSchemas.Contains(schema.Title))
            {
                sb.AppendFormat(" ({0})", context.Type.Namespace);
            }

            schema.Title = sb.ToString();
            _knownSchemas.Add(schema.Title);
        }
    }
}