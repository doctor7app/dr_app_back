using Microsoft.AspNetCore.Http;
using Serilog;

namespace Common.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var handler = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
        var controllerName = handler?.ControllerName ?? "UnknownController";
        var actionName = handler?.ActionName ?? "UnknownAction";

        Log.Information("Starting HTTP {Method} {Path} {QueryString} -> {Controller}.{Action}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            controllerName,
            actionName);

        try
        {
            await next(context);

            Log.Information("Completed HTTP {Method} {Path} -> {Controller}.{Action} with {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                controllerName,
                actionName,
                context.Response.StatusCode);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed HTTP {Method} {Path} -> {Controller}.{Action}",
                context.Request.Method,
                context.Request.Path,
                controllerName,
                actionName);

            throw;
        }
    }
}
