namespace Sourcey.RabbitMQ.Management.Api
{
    public interface IRabbitMqManagementApiClient
    {
        Task<IEnumerable<RabbitMqBinding>> RetrieveSubscriptionsAsync(string queue);
    }
}
