using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Infrastructure.Extension;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Common.Infrastructure.Extension;

public static class MassTransitExtension
{
    public static void AddMassTransit<TConsumer, TDbContext>(
        this IServiceCollection services
        , IConfiguration configuration
        , bool useOutbox = true)
    where TConsumer : IConsumer
    where TDbContext : DbContext
    {
        services.AddMassTransit(x =>
        {
            if (useOutbox)
            {
                x.AddEntityFrameworkOutbox<TDbContext>(o =>
                {
                    o.QueryDelay = TimeSpan.FromSeconds(10);
                    o.UsePostgres();
                    o.UseBusOutbox();
                });
            }

            x.AddConsumersFromNamespaceContaining<TConsumer>();

            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(configuration["MassTransit:Name"], false));

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMq:Host"], "/", h =>
                {
                    h.Username(configuration.GetValue("RabbitMQ:Username", "guest"));
                    h.Password(configuration.GetValue("RabbitMQ:Password", "guest"));
                });

                cfg.ConfigureEndpoints(context);

                cfg.ConfigureJsonSerializerOptions(options =>
                {
                    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true));
                    return options;
                });
            });

        });
    }
}
