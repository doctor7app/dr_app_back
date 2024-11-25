using Common.Extension;
using Common.Middleware;
using Dme.Api.Helpers;
using Microsoft.AspNetCore.OData;
using Dme.Infrastructure.Installation;
using Serilog;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ConfigureSerilog(builder.Configuration["Tracing:Application"], builder.Configuration["Tracing:SeqLoggingURL"])
    .CreateLogger();

builder.Host.UseSerilog();
// Add services to the container.

builder.Services.AddDmeDatabaseServiceCollection();
builder.Services.AddDmeServiceCollection();
builder.Services.AddDmeMassTransitConfig(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddRouting();
builder.Services.AddControllers().AddOData(
    options =>
    {
        options.Select().Filter().OrderBy().Count().SetMaxTop(20).Expand();
        options.EnableQueryFeatures().AddRouteComponents(
            routePrefix: "api",
            model: EdmModelBuilder.Build(),
            configureServices: (services) =>
            {
                services.AddScoped<Microsoft.OData.Json.IJsonWriterFactory>(
                    _ => new Microsoft.OData.Json.ODataJsonWriterFactory());
            });
    });


builder.Services.AddSwaggerConfig();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.UseHttpClientMetrics();

builder.Services.AddOpenTelemetry(builder.Configuration["Tracing:Application"]);


var app = builder.Build();

// Enable Prometheus metrics
app.UseMetricServer();  // Exposes metrics at `/metrics`
app.UseHttpMetrics();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCustomSwaggerConfig();
}

app.UseODataRouteDebug();
app.UseODataQueryRequest();
app.UseODataBatching();

app.UseHttpsRedirection();

//app.UseAuthorization();

app.UseRouting();
app.MapControllers();

app.UseMiddleware<RequestContextLoggingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseInitializeDbDomain();

app.Run();