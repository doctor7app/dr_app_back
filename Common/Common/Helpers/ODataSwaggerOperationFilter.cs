using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.Helpers;

public class ODataSwaggerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var addedParameters = new List<string>();
        if (context.ApiDescription.RelativePath == null) return;

        var relativePath = context.ApiDescription.RelativePath?.TrimStart('/');

        if (relativePath != null && (relativePath.EndsWith("$metadata", StringComparison.OrdinalIgnoreCase) ||
                                     relativePath.EndsWith("api", StringComparison.OrdinalIgnoreCase)))
        {
            operation.Tags.Clear();
            return;
        }

        if (relativePath != null && relativePath.Contains("({key})", StringComparison.OrdinalIgnoreCase))
        {
            operation.Tags.Clear();
            return;
        }

        operation.Parameters.Clear();

        foreach (var param in context.ApiDescription.ActionDescriptor.Parameters)
        {
            if (param.Name.Equals("key", StringComparison.OrdinalIgnoreCase))
            {
                var existingKeyParam = operation.Parameters
                    .FirstOrDefault(p => p.Name.Equals("key", StringComparison.OrdinalIgnoreCase));
                if (existingKeyParam != null)
                {
                    operation.Parameters.Remove(existingKeyParam);
                }

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "key",
                    In = ParameterLocation.Path,
                    Required = true,
                    Schema = new OpenApiSchema { Type = "string", Format = "uuid" }
                });

                addedParameters.Add("key");
            }
            else if (param.Name.Contains("id", StringComparison.OrdinalIgnoreCase))
            {
                if (addedParameters.Contains(param.Name)) continue;

                var existingChildParam = operation.Parameters
                    .FirstOrDefault(p => p.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase));
                if (existingChildParam != null)
                {
                    operation.Parameters.Remove(existingChildParam);
                }

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = param.Name,
                    In = ParameterLocation.Path,
                    Required = true,
                    Schema = new OpenApiSchema { Type = "string", Format = "uuid" }
                });

                addedParameters.Add(param.Name);
            }

            if (!param.ParameterType.IsGenericType ||
                param.ParameterType.GetGenericTypeDefinition() != typeof(Delta<>)) continue;
            var entityType = param.ParameterType.GetGenericArguments().First();
            var schema = GenerateEntitySchema(entityType);

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/json", new OpenApiMediaType { Schema = schema } }
                }
            };
        }
    }

    private OpenApiSchema GenerateEntitySchema(Type entityType)
    {
        var schema = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>()
        };

        foreach (var property in entityType.GetProperties())
        {
            var propertySchema = new OpenApiSchema
            {
                Type = "string" 
            };
                
            if (property.PropertyType.IsEnum)
            {
                propertySchema.Enum = property.PropertyType.GetEnumNames()
                    .Select(x => (IOpenApiAny)new OpenApiString(x))
                    .ToList();
            }
                
            else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
            {
                propertySchema.Type = "string";
                propertySchema.Format = "date-time";
            }
               
            else if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
            {
                propertySchema.Type = "string";
                propertySchema.Format = "uuid";
            }

            schema.Properties.Add(property.Name, propertySchema);
        }

        return schema;
    }
}