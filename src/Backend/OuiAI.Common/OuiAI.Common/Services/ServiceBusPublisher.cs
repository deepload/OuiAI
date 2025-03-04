using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using OuiAI.Common.Interfaces;

namespace OuiAI.Common.Services
{
    public class ServiceBusPublisher : IServiceBusPublisher
    {
        private readonly string _connectionString;
        private readonly ILogger<ServiceBusPublisher> _logger;

        public ServiceBusPublisher(string connectionString, ILogger<ServiceBusPublisher> logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishMessageAsync<T>(string topicName, T message)
        {
            await PublishMessageWithPartitionKeyAsync<T>(topicName, Guid.NewGuid().ToString(), message);
        }

        public async Task PublishMessageWithPartitionKeyAsync<T>(string topicName, string partitionKey, T message)
        {
            try
            {
                // Create a Service Bus client
                await using var client = new ServiceBusClient(_connectionString);
                
                // Create a sender for the topic
                ServiceBusSender sender = client.CreateSender(topicName);

                // Serialize the message to JSON
                string messageJson = JsonSerializer.Serialize(message);
                var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageJson))
                {
                    MessageId = Guid.NewGuid().ToString(),
                    PartitionKey = partitionKey,
                    ContentType = "application/json"
                };

                // Send the message to the topic
                await sender.SendMessageAsync(serviceBusMessage);
                
                _logger.LogInformation($"Message published to topic {topicName} with partition key {partitionKey}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error publishing message to topic {topicName}: {ex.Message}");
                throw;
            }
        }
    }
}
