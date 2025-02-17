using Common.Extension.Services;
using Common.Middleware;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Json;
using Prescriptions.Api.Helpers;
using Prescriptions.Infrastructure.Installation;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ConfigureSerilog(builder.Configuration["Tracing:Application"], builder.Configuration["Tracing:SeqLoggingURL"])
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddPrescriptionDatabaseServiceCollection();
builder.Services.AddPrescriptionServiceCollection();
builder.Services.AddPrescriptionMassTransitConfig(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddRouting();

//builder.Services.AddControllers().AddOData(options =>
//{
//    options.Select().Filter().OrderBy().Count().SetMaxTop(20).Expand();
//    options.EnableQueryFeatures();

//    options.AddRouteComponents(
//        routePrefix: "api/prescriptions",
//        model: EdmModelBuilder.Build(),
//        configureServices: s => s.AddScoped<ODataJsonWriterFactory>()
//    );

//    options.AddRouteComponents(
//        routePrefix: "api/prescriptions/{prescriptionId}/events",
//        model: EdmModelBuilder.GetEventsModel(),
//        configureServices: s => s.AddScoped<ODataJsonWriterFactory>()
//    );
//});

builder.Services.AddControllers().AddOData(
    options =>
    {
        options.Select().Filter().OrderBy().Count().SetMaxTop(20).Expand();
        options.EnableQueryFeatures();
        options.AddRouteComponents(
            routePrefix: "api",
            model: EdmModelBuilder.Build(),
            configureServices: (services) =>
            {
                services.AddScoped<IJsonWriterFactory>(
                    _ => new ODataJsonWriterFactory());
            });
    });

builder.Services.AddSwaggerConfig();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.UseHttpClientMetrics();

builder.Services.AddOpenTelemetry(builder.Configuration["Tracing:Application"]);

var app = builder.Build();

// Enable Prometheus metrics
app.UseMetricServer();
app.UseHttpMetrics();


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