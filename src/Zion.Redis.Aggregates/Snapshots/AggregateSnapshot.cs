using System.Text.Json;
using Microsoft.Extensions.Options;
using Zion.Aggregates;
using Zion.Aggregates.Serialization;
using Zion.Aggregates.Snapshots;
using Zion.Events.Streams;

namespace Zion.Redis.Aggregates.Snapshots
{
    internal class AggregateSnapshot<TAggregate, TState> : IAggregateSnapshot<TAggregate, TState>
        where TAggregate : Aggregate<TState>
        where TState : IAggregateState, new()
    {
        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;
        private readonly IAggregateDeserializer _aggregateDeserializer;
        private readonly IOptionsMonitor<AggregateSnapshotSettings<TState>> _aggregateSnapshotSettings;

        public AggregateSnapshot(IConnectionMultiplexerFactory connectionMultiplexerFactory,
            IAggregateDeserializer aggregateDeserializer,
            IOptionsMonitor<AggregateSnapshotSettings<TState>> aggregateSnapshotSettings)
        {
            if (connectionMultiplexerFactory is null)
                throw new ArgumentNullException(nameof(connectionMultiplexerFactory));
            if (aggregateDeserializer is null)
                throw new ArgumentNullException(nameof(aggregateDeserializer));
            if (aggregateSnapshotSettings is null)
                throw new ArgumentNullException(nameof(aggregateSnapshotSettings));

            _connectionMultiplexerFactory = connectionMultiplexerFactory;
            _aggregateDeserializer = aggregateDeserializer;
            _aggregateSnapshotSettings = aggregateSnapshotSettings;
        }

        public async Task<TAggregate?> GetAsync(StreamId stream, Func<TAggregate, CancellationToken, Task<TAggregate>> rehydrateAsync, CancellationToken cancellationToken = default)
        {
            var connection = _connectionMultiplexerFactory.Create(_aggregateSnapshotSettings.CurrentValue.ConnectionString);
            var db = connection.GetDatabase();

            if (!await db.KeyExistsAsync(stream.ToString()))
                return null;

            var value = (await db.StringGetAsync(stream.ToString())).ToString();

            if (string.IsNullOrWhiteSpace(value))
                return null;

            return _aggregateDeserializer.Deserialize<TAggregate>(value);
        }
    }
}
