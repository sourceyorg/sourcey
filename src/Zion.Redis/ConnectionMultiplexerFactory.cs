using System.Collections.Concurrent;
using StackExchange.Redis;

namespace Zion.Redis
{
    internal sealed class ConnectionMultiplexerFactory : IConnectionMultiplexerFactory
    {
        private readonly ConcurrentDictionary<string, IConnectionMultiplexer> _multiplexerCache = new();

        public IConnectionMultiplexer Create(string connectionString)
            => _multiplexerCache.GetOrAdd(connectionString, key => ConnectionMultiplexer.Connect(key));
    }
}
