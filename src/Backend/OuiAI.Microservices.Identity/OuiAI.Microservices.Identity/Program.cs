using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OuiAI.Common.Extensions;
using OuiAI.Microservices.Identity.Data;
using OuiAI.Microservices.Identity.Models;
using OuiAI.Microservices.Identity.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();

// Add Common Services from Extensions
builder.Services.AddCommonJwtAuthentication(builder.Configuration);
builder.Services.AddCommonCorsPolicy();
builder.Services.AddSwaggerWithJwt("OuiAI Identity API");
builder.Services.AddServiceBusPublisher(builder.Configuration);

// Register identity service
builder.Services.AddScoped<IIdentityService, IdentityService>();

var app = builder.Build();

// Configure the HTTP request pipeline with common middleware
app.UseCommonMiddleware(app.Environment);

app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.MigrateAndSeedDatabaseAsync<IdentityDbContext, Program>(async context => 
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        
        // Seed roles
        if (!roleManager.Roles.Any())
        {
            await roleManager.CreateAsync(new ApplicationRole("Admin") { Description = "Administrator role with full access" });
            await roleManager.CreateAsync(new ApplicationRole("User") { Description = "Standard user role" });
            await roleManager.CreateAsync(new ApplicationRole("Moderator") { Description = "Moderator role for content management" });
        }
        
        // Seed admin user if no users exist
        if (!userManager.Users.Any())
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@ouiai.com",
                CreatedAt = DateTime.UtcNow
            };
            
            await userManager.CreateAsync(adminUser, "Admin@123");
            await userManager.AddToRoleAsync(adminUser, "Admin");
            
            // Create admin profile
            context.UserProfiles.Add(new UserProfile
            {
                UserId = adminUser.Id,
                DisplayName = "Admin",
                Bio = "Platform Administrator",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            
            await context.SaveChangesAsync();
        }
    });
}

app.Run();
