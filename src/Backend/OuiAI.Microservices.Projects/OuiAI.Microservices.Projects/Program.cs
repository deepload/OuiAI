using Microsoft.EntityFrameworkCore;
using OuiAI.Common.Extensions;
using OuiAI.Microservices.Projects.Data;
using OuiAI.Microservices.Projects.Interfaces;
using OuiAI.Microservices.Projects.Models;
using OuiAI.Microservices.Projects.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<ProjectsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProjectsConnection")));

// Add Common Services from Extensions
builder.Services.AddCommonJwtAuthentication(builder.Configuration);
builder.Services.AddCommonCorsPolicy();
builder.Services.AddSwaggerWithJwt("OuiAI Projects API");
builder.Services.AddServiceBusPublisher(builder.Configuration);

// Register Project specific services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IMediaService, MediaService>();

var app = builder.Build();

// Configure the HTTP request pipeline with common middleware
app.UseCommonMiddleware(app.Environment);

app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.MigrateAndSeedDatabaseAsync<ProjectsDbContext, Program>(async context => 
    {
        // Seed initial categories if none exist
        if (!context.Categories.Any())
        {
            var categories = new[]
            {
                new ProjectCategory
                {
                    Id = Guid.NewGuid(),
                    Name = "Computer Vision",
                    Description = "Projects focused on image and video analysis, object detection, and recognition",
                    IconUrl = "/images/categories/computer-vision.png",
                    DisplayOrder = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new ProjectCategory
                {
                    Id = Guid.NewGuid(),
                    Name = "Natural Language Processing",
                    Description = "Projects focused on text analysis, language understanding, and generation",
                    IconUrl = "/images/categories/nlp.png",
                    DisplayOrder = 2,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new ProjectCategory
                {
                    Id = Guid.NewGuid(),
                    Name = "Generative AI",
                    Description = "Projects focused on generating content like images, text, music, etc.",
                    IconUrl = "/images/categories/generative.png",
                    DisplayOrder = 3,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new ProjectCategory
                {
                    Id = Guid.NewGuid(),
                    Name = "Reinforcement Learning",
                    Description = "Projects focused on developing agents that learn through interaction with environments",
                    IconUrl = "/images/categories/reinforcement.png",
                    DisplayOrder = 4,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new ProjectCategory
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
    });
}

app.Run();
