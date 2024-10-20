using DocumentManagement.Core.Interfaces.Data;
using DocumentManagement.Core.Interfaces.Services;
using DocumentManagement.Core.Services;
using DocumentManagement.Core.Services.Storage;
using DocumentManagement.Infrastructure.Data;
using DocumentManagement.Infrastructure.Repositories;
using DocumentManagement.Infrastructure.Logging;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Set up Serilog for logging
var loggerConfiguration = SerilogConfig.CreateLoggerConfiguration();
Log.Logger = loggerConfiguration
    .ReadFrom.Configuration(builder.Configuration) // This allows overrides from appsettings.json
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add PostgreSQL DbContext
builder.Services.AddDbContext<DocumentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Document Service and Storage Service
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IStorageService, LocalStorageService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IMetadataRepository, MetadataRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

var app = builder.Build();

// Enable Prometheus metrics
app.UseMetricServer();  // Exposes metrics at `/metrics`

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Add Serilog middleware to capture request and response logs
app.UseSerilogRequestLogging();

// init database
DbInitializer.InitDb(app);

// Start the application
app.Run();