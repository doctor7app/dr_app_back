using Common.Infrastructure.Installation;
using Notify.Consumers;
using Notify.Core.Interfaces.Data;
using Notify.Core.Interfaces.Services;
using Notify.Core.Services;
using Notify.Infrastructure.Data;
using Notify.Infrastructure.Repositories;
using Notify.Worker;
using Notify.Core.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseCommonSerilog(builder.Configuration);

builder.Services.AddCommonServices<NotifyConsumer, DataDbContext>(builder.Configuration, typeof(Program).Assembly);

// Add Services
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotifyService, NotifyService>();
builder.Services.AddScoped<INotifyItemRepository, NotifyItemRepository>();
builder.Services.AddHostedService<MailWorker>();

var app = builder.Build();

app.UseCommon();

// init database
DbInitializer.InitDb(app);

app.Run();
