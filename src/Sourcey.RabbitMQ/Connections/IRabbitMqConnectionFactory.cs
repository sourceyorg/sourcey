namespace Sourcey.RabbitMQ.Connections
{
    public interface IRabbitMqConnectionFactory
    {
        Task<IRabbitMqConnection> CreateConnectionAsync(CancellationToken cancellationToken);
    }
}
