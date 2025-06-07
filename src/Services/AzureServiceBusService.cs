using System.Text.Json;
using Azure.Messaging.ServiceBus;
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

        private readonly string _connectionString;
        private ServiceBusClient? _client;

        public AzureServiceBusService(IOptions<AzureServiceBusSettings> azureServiceBusOptions, ILogger<AzureServiceBusService> logger)
        {
            _settings = azureServiceBusOptions.Value;
            _logger = logger;
        }

        public async Task SendMessageAsync(string queueName, string message)
        {
            EnsureClient();
            await using var sender = _client!.CreateSender(queueName);
            await sender.SendMessageAsync(new ServiceBusMessage(message));
        }

        public async Task SubscribeAsync(string queueName, Func<string, Task> callback)
        {
            EnsureClient();

            await using var receiver = _client!.CreateReceiver(queueName);

            var message = await receiver.ReceiveMessageAsync();
            if (message != null)
            {
                await receiver.CompleteMessageAsync(message);
            }

            await callback(message?.Body?.ToString());
        }

        private void EnsureClient()
        {
            if (_client == null || _client.IsClosed)
            {
                _client = new ServiceBusClient(_connectionString);
            }
        }

        public Task PublishAsync(object data, string subscription, string key = "")
        {
            throw new NotImplementedException();
        }

        public Task SubscribeAsync(string subscription, string queueName, Func<string, Task> callback, string key = "")
        {
            throw new NotImplementedException();
        }

        public Task SendMessageAsync(string queueName, object data)
        {
            throw new NotImplementedException();
        }

        public async Task SubscribeAsync<T>(string queueName, Func<T, Task> callback) where T : class
        {
            EnsureClient();

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
    }
}