using Microsoft.AspNetCore.OData;
using Patients.Api.Extensions;
using Patients.Api.Handler;
using Patients.Api.Helpers;
using Patients.Api.Middleware;
using Patients.Infrastructure.Installation;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ConfigureSerilog(builder.Configuration["Tracing:Application"], builder.Configuration["Tracing:SeqLoggingURL"])
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddPatientDatabaseServiceCollection();
builder.Services.AddPatientServiceCollection();

builder.Services.AddControllers();
builder.Services.AddRouting();
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseODataRouteDebug();
app.UseODataQueryRequest();
app.UseODataBatching();

app.UseCustomSwaggerConfig();

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
