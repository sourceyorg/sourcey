using StackExchange.Redis;

namespace Sourcey.Redis
{
    public interface IConnectionMultiplexerFactory
    {
        IConnectionMultiplexer Create(string connectionString);
    }
}
