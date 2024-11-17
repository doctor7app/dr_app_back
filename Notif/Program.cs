using Common.Infrastructure.Installation;
using Notif.Consumers;
using Notif.Core.Interfaces.Services;
using Notif.Core.Services;
using Notif.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseCommonSerilog(builder.Configuration);

builder.Services.AddCommonServices<NotifConsumer, DataDbContext>(builder.Configuration);

// Add Services
builder.Services.AddScoped<INotifService, NotifService>();

var app = builder.Build();

app.UseCommon();

// init database
DbInitializer.InitDb(app);

app.Run();
