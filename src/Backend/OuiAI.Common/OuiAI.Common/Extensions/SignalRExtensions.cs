using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace OuiAI.Common.Extensions
{
    public static class SignalRExtensions
    {
        /// <summary>
        /// Adds SignalR with common configuration used across microservices
        /// </summary>
        public static IServiceCollection AddCommonSignalR(this IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                // Set a longer timeout for better connection stability
                options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
                options.KeepAliveInterval = TimeSpan.FromMinutes(1);
                
                // Enable detailed errors
                options.EnableDetailedErrors = true;
            })
            .AddJsonProtocol(options =>
            {
                // Use camel case for properties
                options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });
            
            return services;
        }
        
        /// <summary>
        /// Configures SignalR for Azure Service Bus backplane in a clustered environment
        /// </summary>
        public static IServiceCollection AddSignalRWithServiceBus(this IServiceCollection services, string connectionString)
        {
            services.AddCommonSignalR()
                .AddAzureSignalR(options =>
                {
                    options.ConnectionString = connectionString;
                    options.GracefulShutdown.Mode = Microsoft.Azure.SignalR.GracefulShutdownMode.WaitForClientsClose;
                    options.GracefulShutdown.Timeout = TimeSpan.FromSeconds(30);
                });
                
            return services;
        }
    }
}
