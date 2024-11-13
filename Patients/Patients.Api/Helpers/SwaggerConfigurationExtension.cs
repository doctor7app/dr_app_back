using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Patients.Api.Helpers
{
    public static class SwaggerConfigurationExtension
    {
        public static void AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Patient Api",
                    Description = "Patient Api",
                    Contact = new OpenApiContact
                    {
                        Name = "Hamza Ziadi",
                        Email = string.Empty,
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {new OpenApiSecurityScheme{Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    }}, new string[] {}}
                });
                c.OperationFilter<ODataSwaggerOperationFilter>();
            });
        }

        public static void UseCustomSwaggerConfig(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Patient API V1");
            });
        }
    }

    public class ODataSwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var addedParameters = new List<string>();
            if (context.ApiDescription.RelativePath == null ||
                !context.ApiDescription.RelativePath.Contains("(")) return;
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
            }

            if (operation.Tags == null || !operation.Tags.Contains(new OpenApiTag { Name = "api" })) return;
            var path = context.ApiDescription.RelativePath;
            if (path.Contains("({key})"))
            {
                operation.Summary += " (Entity)";
            }
        }
    }

}