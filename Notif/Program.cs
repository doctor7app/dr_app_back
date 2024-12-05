using Common.Infrastructure.Installation;
using Notif.Consumers;
using Notif.Core.Interfaces.Data;
using Notif.Core.Interfaces.Services;
using Notif.Core.Services;
using Notif.Infrastructure.Data;
using Notif.Infrastructure.Repositories;
using Notif.Worker;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseCommonSerilog(builder.Configuration);

builder.Services.AddCommonServices<NotifConsumer, DataDbContext>(builder.Configuration, AppDomain.CurrentDomain.GetAssemblies());

// Add Services
builder.Services.AddScoped<INotifService, NotifService>();
builder.Services.AddScoped<INotifItemRepository, NotifItemRepository>();
builder.Services.AddHostedService<MailWorker>();

var app = builder.Build();

app.UseCommon();

// init database
DbInitializer.InitDb(app);

app.Run();
