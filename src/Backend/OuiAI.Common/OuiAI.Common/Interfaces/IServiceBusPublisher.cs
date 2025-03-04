using System.Threading.Tasks;

namespace OuiAI.Common.Interfaces
{
    public interface IServiceBusPublisher
    {
        Task PublishMessageAsync<T>(string topicName, T message);
        Task PublishMessageWithPartitionKeyAsync<T>(string topicName, string partitionKey, T message);
    }
}
