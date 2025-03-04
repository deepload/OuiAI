using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OuiAI.Common.Services
{
    public abstract class ServiceBusSubscriber<TMessage> : BackgroundService
    {
        private readonly string _connectionString;
        private readonly string _topicName;
        private readonly string _subscriptionName;
        private readonly ILogger _logger;
        private ServiceBusProcessor _processor;

        protected ServiceBusSubscriber(
            string connectionString,
            string topicName,
            string subscriptionName,
            ILogger logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _topicName = topicName ?? throw new ArgumentNullException(nameof(topicName));
            _subscriptionName = subscriptionName ?? throw new ArgumentNullException(nameof(subscriptionName));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Starting ServiceBusSubscriber for topic {_topicName} and subscription {_subscriptionName}");

            // Create client and processor
            var client = new ServiceBusClient(_connectionString);
            _processor = client.CreateProcessor(_topicName, _subscriptionName, new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 10,
                AutoCompleteMessages = false
            });

            // Configure handlers
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            // Start processing
            await _processor.StartProcessingAsync(stoppingToken);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                // Expected when stoppingToken is canceled
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in ServiceBusSubscriber for topic {_topicName}: {ex.Message}");
            }
            finally
            {
                await _processor.StopProcessingAsync();
                await _processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            try
            {
                string messageBody = Encoding.UTF8.GetString(args.Message.Body);
                _logger.LogInformation($"Received message: {messageBody}");

                var message = JsonSerializer.Deserialize<TMessage>(messageBody);
                
                // Process the message
                await ProcessMessageAsync(message, args.Message.MessageId, args.CancellationToken);
                
                // Complete the message
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing message: {ex.Message}");
                await args.AbandonMessageAsync(args.Message);
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, $"Error in message processing: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Process the received message. This method must be implemented by derived classes.
        /// </summary>
        /// <param name="message">The deserialized message.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        protected abstract Task ProcessMessageAsync(TMessage message, string messageId, CancellationToken cancellationToken);
    }
}
