using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OuiAI.Common.Interfaces;
using OuiAI.Common.Services;
using OuiAI.Microservices.Projects.Data;
using OuiAI.Microservices.Projects.Interfaces;
using OuiAI.Microservices.Projects.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<ProjectsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProjectsConnection")));

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// Register Services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IMediaService, MediaService>();

// Add Service Bus
builder.Services.AddSingleton<IServiceBusPublisher>(provider =>
    new ServiceBusPublisher(
        builder.Configuration.GetConnectionString("ServiceBus"),
        provider.GetRequiredService<ILogger<ServiceBusPublisher>>()));

// Add Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OuiAI Projects API", Version = "v1" });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("CorsPolicy");

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ProjectsDbContext>();
        
        // Apply migrations if they don't exist
        context.Database.Migrate();
        
        // Seed initial categories if none exist
        if (!context.Categories.Any())
        {
            var categories = new[]
            {
                new OuiAI.Microservices.Projects.Models.ProjectCategory
                {
                    Id = Guid.NewGuid(),
                    Name = "Computer Vision",
                    Description = "Projects focused on image and video analysis, object detection, and recognition",
                    IconUrl = "/images/categories/computer-vision.png",
                    DisplayOrder = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new OuiAI.Microservices.Projects.Models.ProjectCategory
                {
                    Id = Guid.NewGuid(),
                    Name = "Natural Language Processing",
                    Description = "Projects focused on text analysis, language understanding, and generation",
                    IconUrl = "/images/categories/nlp.png",
                    DisplayOrder = 2,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new OuiAI.Microservices.Projects.Models.ProjectCategory
                {
                    Id = Guid.NewGuid(),
                    Name = "Generative AI",
                    Description = "Projects focused on generating content like images, text, music, etc.",
                    IconUrl = "/images/categories/generative.png",
                    DisplayOrder = 3,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new OuiAI.Microservices.Projects.Models.ProjectCategory
                {
                    Id = Guid.NewGuid(),
                    Name = "Reinforcement Learning",
                    Description = "Projects focused on developing agents that learn through interaction with environments",
                    IconUrl = "/images/categories/reinforcement.png",
                    DisplayOrder = 4,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new OuiAI.Microservices.Projects.Models.ProjectCategory
                {
                    Id = Guid.NewGuid(),
                    Name = "AI Tools",
                    Description = "Projects focused on developer tools and utilities for AI",
                    IconUrl = "/images/categories/tools.png",
                    DisplayOrder = 5,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
            
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
