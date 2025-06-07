namespace KinoDev.Shared.Services.Abstractions
{
    public interface IMessageBrokerService
    {
        Task PublishAsync(object data, string subscription, string key = "");

        Task SubscribeAsync(string subscription, string queueName, Func<string, Task> callback, string key = "");

        Task SendMessageAsync(string queueName, string message);

        Task<string> ReceiveMessageAsync(string queueName, CancellationToken cancellationToken = default);
    } 
}