using System.Text;
using System.Text.Json;
using KinoDev.Shared.Models;
using KinoDev.Shared.Services.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace KinoDev.Shared.Services
{
    public class RabbitMQService : IMessageBrokerService
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

        public async Task SendMessageAsync(string queueName, object data)
        {
            await EnsureConnection(queueName);

            await _channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var message = JsonSerializer.Serialize(data);
            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: false,
                body: body
            );
        }

        public async Task SubscribeAsync<T>(string queueName, Func<T, Task> callback) where T : class
        {
            await EnsureConnection(queueName);

            await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var data = JsonSerializer.Deserialize<T>(message);
                await callback(data);
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