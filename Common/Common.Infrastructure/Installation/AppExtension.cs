using Common.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Serilog;

namespace Common.Infrastructure.Installation
{
    public static class AppExtension
    {
        public static IApplicationBuilder UseCommon(this IApplicationBuilder app)
        {
            var environment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

            app.UseExceptionHandler();

            app.UseHttpsRedirection();

            app.UseMiddleware<RequestContextLoggingMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseSerilogRequestLogging();

            // Enable Prometheus metrics
            app.UseMetricServer();
            // Exposes metrics at `/metrics`
            app.UseHttpMetrics(); 

            if (environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            return app;
        }

    }
}