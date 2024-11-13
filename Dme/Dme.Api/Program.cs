using Dme.Api.Helpers;
using Microsoft.AspNetCore.OData;
using Dme.Infrastructure.Installation;
using Serilog;
using Dme.Api.Extensions;
using Dme.Api.Middleware;
using Prometheus;
using Dme.Api.Handler;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ConfigureSerilog(builder.Configuration["Tracing:Application"], builder.Configuration["Tracing:SeqLoggingURL"])
    .CreateLogger();

builder.Host.UseSerilog();
// Add services to the container.

builder.Services.AddDmeDatabaseServiceCollection();
builder.Services.AddDmeServiceCollection();

builder.Services.AddControllers();

builder.Services.AddRouting();
builder.Services.AddControllers().AddNewtonsoftJson().AddOData(opt =>
{
    opt.Select().Filter().OrderBy().Count().SetMaxTop(20).Expand();
    opt.AddRouteComponents("api", EdmModelBuilder.Build());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
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

//app.UseExceptionHandler();

app.UseInitializeDbDomain();

app.Run();