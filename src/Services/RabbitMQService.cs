using System.Text;
using KinoDev.Shared.Models;
using KinoDev.Shared.Services.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace KinoDev.Shared.Services
{
    public class RabbitMQService : IMessageBrokerService, IAsyncDisposable
    {
        private IConnection _connection;

        private IChannel _channel;

        private readonly ILogger<RabbitMQService> _logger;

        private readonly RabbitMqSettings _settings;

        public RabbitMQService(IOptions<RabbitMqSettings> rabbitMqOptions, ILogger<RabbitMQService> logger)
        {
            _logger = logger;
            _settings = rabbitMqOptions.Value;
        }

        public async Task PublishAsync(object data, string subscription, string key = "")
        {
            await ValdiateConnectionState(subscription);

            var message = System.Text.Json.JsonSerializer.Serialize(data);
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                subscription,
                key,
                body: body,
                mandatory: false
            );
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.DisposeAsync();
                _channel = null;
            }

            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        private async Task ValdiateConnectionState(string exchange)
        {
            var validateExchange = false;

            if (_connection == null || !_connection.IsOpen)
            {
                try
                {
                    var factory = new ConnectionFactory
                    {
                        HostName = _settings.HostName,
                        Port = _settings.Port,
                        UserName = _settings.UserName,
                        Password = _settings.Password,
                    };

                    // Close the existing channel if it's open
                    if (_channel != null && _channel.IsOpen)
                    {
                        await _channel.CloseAsync();
                    }

                    _connection = await factory.CreateConnectionAsync();
                    _channel = await _connection.CreateChannelAsync();
                    validateExchange = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to connect to RabbitMQ.");
                    throw;
                }
            }
            else if (_channel == null || !_channel.IsOpen)
            {
                try
                {
                    _channel = await _connection.CreateChannelAsync();
                    validateExchange = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create channel.");
                    throw;
                }
            }

            if (validateExchange)
            {
                // TODO: Move to separate method as we can have multiple exchanges in the future
                await _channel.ExchangeDeclareAsync(
                    exchange: exchange,
                    type: ExchangeType.Fanout,
                    durable: true,
                    autoDelete: false,
                    arguments: null
                );
            }
        }

        public async Task SubscribeAsync(string subscription, string queueName, Func<string, Task> callback, string key = "")
        {
            await ValdiateConnectionState(subscription);

            var queue = await _channel.QueueDeclareAsync(queueName);
            await _channel.QueueBindAsync(
                queue: queue,
                exchange: subscription,
                routingKey: key
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await callback(message);
            };

            await _channel.BasicConsumeAsync(
                queue: queue,
                autoAck: true,
                consumer: consumer
            );
        }

        public async Task SendMessageAsync(string queueName, string message)
        {
            await EnsureConnection(queueName);

            await _channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: false,
                body: body
            );
        }

        public async Task SubscribeAsync(string queueName, Func<string, Task> callback)
        {
            await EnsureConnection(queueName);

            await _channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await callback(message);           
            };

            await _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
        }

        private async Task EnsureConnection(string exchange)
        {
            var restoreConnection = false;

            if (_connection == null || !_connection.IsOpen)
            {
                restoreConnection = true;
            }
            else if (_channel == null || !_channel.IsOpen)
            {
                restoreConnection = true;
            }
            
            if (restoreConnection)
            {
                if (_connection != null)
                {
                    await _connection.DisposeAsync();
                }

                if (_channel != null)
                {
                    await _channel.DisposeAsync();
                }

                var factory = new ConnectionFactory
                {
                    HostName = _settings.HostName,
                    Port = _settings.Port,
                    UserName = _settings.UserName,
                    Password = _settings.Password,
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
            }
        }
    }
}