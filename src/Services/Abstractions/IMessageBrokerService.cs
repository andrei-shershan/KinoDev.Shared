namespace KinoDev.Shared.Services.Abstractions
{
    public interface IMessageBrokerService
    {
        Task PublishAsync(object data, string subscription, string key = "");

        Task SubscribeAsync(string subscription, string queueName, Func<string, Task> callback, string key = "");

        Task SendMessageAsync(string queueName, object data);

        Task SubscribeAsync(string queueName, Func<string, Task> callback);

        Task SubscribeAsync<T>(string queueName, Func<T, Task> callback) 
        where T : class;
    }
}