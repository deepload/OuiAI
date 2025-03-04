using OuiAI.Common.Extensions;
using OuiAI.Microservices.Gateway.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Common Services from Extensions
builder.Services.AddCommonCorsPolicy();
builder.Services.AddSwaggerWithJwt("OuiAI Gateway API");

// Register Services
builder.Services.AddHttpClient();
builder.Services.AddScoped<IApiGatewayService, ApiGatewayService>();

// Configure the HTTP cache headers
builder.Services.AddResponseCaching();

var app = builder.Build();

// Configure the HTTP request pipeline with common middleware
app.UseCommonMiddleware(app.Environment);

// Use response caching for improved performance
app.UseResponseCaching();

app.MapControllers();

app.Run();
