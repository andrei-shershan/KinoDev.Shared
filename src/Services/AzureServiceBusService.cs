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

                await using var receiver = _client!.CreateReceiver(queueName);

                var message = await receiver.ReceiveMessageAsync();
                if (message != null)
                {
                    await receiver.CompleteMessageAsync(message);
                }

                // Deserialize the message body to the specified type T
                var data = JsonSerializer.Deserialize<T>(message.Body.ToString());

                await callback(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing to queue: {QueueName}", queueName);
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