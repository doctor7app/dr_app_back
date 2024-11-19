using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.Helpers.Swagger;

public class RemoveODataPathsDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var pathsToRemove = new[] { "$metadata", "api", "({key})", "$count" };
        foreach (var path in swaggerDoc.Paths.ToList())
        {
            if (pathsToRemove.Any(removePath => path.Key.EndsWith(removePath, StringComparison.OrdinalIgnoreCase)))
            {
                swaggerDoc.Paths.Remove(path.Key);
            }
        }
    }
}