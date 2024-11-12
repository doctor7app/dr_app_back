using Dme.Api.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dme.Infrastructure.Installation;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

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

//app.UseExceptionHandler();

app.UseInitializeDbDomain();

app.Run();