using Microsoft.EntityFrameworkCore;
using OuiAI.Common.Extensions;
using OuiAI.Microservices.Notifications.Data;
using OuiAI.Microservices.Notifications.Interfaces;
using OuiAI.Microservices.Notifications.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<NotificationsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NotificationsConnection")));

// Add Common Services from Extensions
builder.Services.AddCommonJwtAuthentication(builder.Configuration);
builder.Services.AddCommonCorsPolicy();
builder.Services.AddSwaggerWithJwt("OuiAI Notifications API");
builder.Services.AddServiceBusPublisher(builder.Configuration);

// Register Notifications specific services
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IPushNotificationService, PushNotificationService>();

// Add hosted services for background processing
builder.Services.AddHostedService<EmailNotificationProcessor>();
builder.Services.AddHostedService<SmsNotificationProcessor>();
builder.Services.AddHostedService<PushNotificationProcessor>();

var app = builder.Build();

// Configure the HTTP request pipeline with common middleware
app.UseCommonMiddleware(app.Environment);

app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.MigrateAndSeedDatabaseAsync<NotificationsDbContext, Program>();
}

app.Run();
