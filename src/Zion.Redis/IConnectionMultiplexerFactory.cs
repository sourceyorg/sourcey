using StackExchange.Redis;

namespace Zion.Redis
{
    public interface IConnectionMultiplexerFactory
    {
        IConnectionMultiplexer Create(string connectionString);
    }
}
