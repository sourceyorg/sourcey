namespace Zion.RabbitMQ.Connections
{
    internal class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
    {
        private readonly RabbitMqConnectionPool _pool;

        public RabbitMqConnectionFactory(RabbitMqConnectionPool pool)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            _pool = pool;
        }

        public async Task<IRabbitMqConnection> CreateConnectionAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Yield();

            var connection = _pool.GetConnection();

            return connection;
        }
    }
}
