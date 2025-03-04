using Microsoft.EntityFrameworkCore;
using OuiAI.Common.Extensions;
using OuiAI.Microservices.Social.Data;
using OuiAI.Microservices.Social.Hubs;
using OuiAI.Microservices.Social.Interfaces;
using OuiAI.Microservices.Social.Mapping;
using OuiAI.Microservices.Social.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<SocialDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SocialConnection")));

// Add Common Services from Extensions
builder.Services.AddCommonJwtAuthentication(builder.Configuration);
builder.Services.AddCommonCorsPolicy();
builder.Services.AddSwaggerWithJwt("OuiAI Social API");
builder.Services.AddServiceBusPublisher(builder.Configuration);

// Register Social specific services
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IMessageService, MessageService>();

// Add SignalR with common configuration
builder.Services.AddCommonSignalR();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline with common middleware
app.UseCommonMiddleware(app.Environment);

app.MapControllers();

// Map SignalR hubs
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapHub<ConversationHub>("/hubs/conversations");

// Initialize database
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.MigrateAndSeedDatabaseAsync<SocialDbContext, Program>();
}

app.Run();
