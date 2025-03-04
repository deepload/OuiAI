using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace OuiAI.Common.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Configures the standard middleware pipeline for all microservices
        /// </summary>
        public static IApplicationBuilder UseCommonMiddleware(this IApplicationBuilder app, IHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
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

            return app;
        }
    }
}
