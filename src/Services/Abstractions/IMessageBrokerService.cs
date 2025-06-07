namespace KinoDev.Shared.Services.Abstractions
{
    public interface IMessageBrokerService
    {
        Task SendMessageAsync(string queueName, object data);

        Task SubscribeAsync<T>(string queueName, Func<T, Task> callback) where T : class;
    }
}