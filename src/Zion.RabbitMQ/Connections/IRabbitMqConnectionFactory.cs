namespace Zion.RabbitMQ.Connections
{
    public interface IRabbitMqConnectionFactory
    {
        Task<IRabbitMqConnection> CreateConnectionAsync(CancellationToken cancellationToken);
    }
}
