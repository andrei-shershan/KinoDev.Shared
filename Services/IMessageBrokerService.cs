namespace KinoDev.Shared.Services
{
    public interface IMessageBrokerService
    {
        Task PublishAsync(object data, string subscription, string key = "");

        Task SubscribeAsync(string subscription, string queueName, Func<string, Task> callback, string key = "");
    } 
}