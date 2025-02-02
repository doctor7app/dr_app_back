using Common.Infrastructure.Extension;
using Common.Infrastructure.Handler;
using Common.Infrastructure.Pipeline;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using System.Reflection;

namespace Common.Infrastructure.Installation
{
    public static class AddServicesExtension
    {
        public static IServiceCollection AddCommonServices<TConsumer, TDbContext>(
            this IServiceCollection services
            , IConfiguration configuration
            , Assembly assembly
            , bool pipelineLogging = true
            )
            where TConsumer : IConsumer
            where TDbContext : DbContext
        {
            // Add services to the container.
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Add PostgreSQL DbContext
            services.AddDbContext<TDbContext>(configuration);

            // Add AutoMapper
            services.AddAutoMapper(assembly);

            // MassTransit Configuration (RabbitMQ)
            services.AddMassTransit<TConsumer, TDbContext>(configuration);

            // Add logging with MediatR
            if (pipelineLogging)
            {
                services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestLoggingPipelineBehavior<,>));
            }

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.UseHttpClientMetrics();
            services.AddOpenTelemetry(configuration["Tracing:Application"]);

            return services;
        }

        public static IServiceCollection AddCommonServices<TDbContext>(
            this IServiceCollection services
            , IConfiguration configuration
            , bool pipelineLogging = true
            )
            where TDbContext : DbContext
        {
            // Add services to the container.
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Add PostgreSQL DbContext
            services.AddDbContext<TDbContext>(configuration);
            // Add logging with MediatR
            if (pipelineLogging)
            {
                services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestLoggingPipelineBehavior<,>));
            }

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.UseHttpClientMetrics();
            services.AddOpenTelemetry(configuration["Tracing:Application"]);

            return services;
        }

    }
}