using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OuiAI.Common.Extensions
{
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Applies database migrations and optionally runs a seeding action
        /// </summary>
        public static async Task MigrateAndSeedDatabaseAsync<TContext, TProgram>(
            this IServiceProvider serviceProvider, 
            Func<TContext, Task> seedAction = null) 
            where TContext : DbContext
            where TProgram : class
        {
            try
            {
                var context = serviceProvider.GetRequiredService<TContext>();
                
                // Apply migrations if they don't exist
                await context.Database.MigrateAsync();
                
                // Execute seeding logic if provided
                if (seedAction != null)
                {
                    await seedAction(context);
                }
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<TProgram>>();
                logger.LogError(ex, "An error occurred while migrating or seeding the database.");
            }
        }
    }
}
