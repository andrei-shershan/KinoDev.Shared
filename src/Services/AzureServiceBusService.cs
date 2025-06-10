using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using KinoDev.Shared.Models;
using KinoDev.Shared.Services.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KinoDev.Shared.Services
{
    public class AzureServiceBusService : IMessageBrokerService
    {
        private readonly AzureServiceBusSettings _settings;
        private readonly ILogger<AzureServiceBusService> _logger;
        private ServiceBusClient? _client;
        private readonly ServiceBusAdministrationClient _adminClient;

        public AzureServiceBusService(IOptions<AzureServiceBusSettings> azureServiceBusOptions, ILogger<AzureServiceBusService> logger)
        {
            _settings = azureServiceBusOptions.Value;

            _adminClient = new ServiceBusAdministrationClient(_settings.ConnectionString);

            _logger = logger;
        }

        public async Task SendMessageAsync(string queueName, object data)
        {
            EnsureClient();
            await EnsureQueueExistsAsync(queueName);
            await using var sender = _client!.CreateSender(queueName);

            var message = JsonSerializer.Serialize(data);
            await sender.SendMessageAsync(new ServiceBusMessage(message));
        }

        public async Task SubscribeAsync<T>(string queueName, Func<T, Task> callback) where T : class
        {
              try
            {
                EnsureClient();
                await EnsureQueueExistsAsync(queueName);            
                
                var processor = _client!.CreateProcessor(queueName, new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = 1 // Adjust as needed
                });

                processor.ProcessMessageAsync += async args =>
                {
                    try
                    {
                        var data = JsonSerializer.Deserialize<T>(args.Message.Body.ToString());
                        if (data != null)
                        {
                            await callback(data);
                        }

                        await args.CompleteMessageAsync(args.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error processing message from queue: {QueueName}", queueName);
                        await args.AbandonMessageAsync(args.Message);
                    }
                };

                processor.ProcessErrorAsync += async args =>
                {
                    _logger?.LogError(args.Exception, "Error in message processor for queue: {QueueName}", queueName);
                    await Task.CompletedTask;
                };

                await processor.StartProcessingAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error subscribing to queue: {QueueName}", queueName);
                throw;
            }
        }

        private void EnsureClient()
        {
            if (_client == null || _client.IsClosed)
            {
                _client = new ServiceBusClient(_settings.ConnectionString);
            }
        }

        private async Task EnsureQueueExistsAsync(string queueName)
        {
            try
            {
                if (!await _adminClient.QueueExistsAsync(queueName))
                {
                    _logger.LogInformation("Creating queue: {QueueName}", queueName);
                    await _adminClient.CreateQueueAsync(queueName);
                    _logger.LogInformation("Queue created: {QueueName}", queueName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ensuring queue exists: {QueueName}", queueName);
                throw;
            }
        }
    }
}