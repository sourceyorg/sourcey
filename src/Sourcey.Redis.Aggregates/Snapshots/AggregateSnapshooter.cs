using System.Text.Json;
using Microsoft.Extensions.Options;
using Sourcey.Aggregates;
using Sourcey.Aggregates.Serialization;
using Sourcey.Aggregates.Snapshots;

namespace Sourcey.Redis.Aggregates.Snapshots
{
    internal class AggregateSnapshooter<TState> : IAggregateSnapshooter<TState>
        where TState : IAggregateState, new()
    {
        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;
        private readonly IAggregateSerializer _aggregateSerializer;
        private readonly IOptionsMonitor<AggregateSnapshotSettings<TState>> _aggregateSnapshotSettings;

        public AggregateSnapshooter(IConnectionMultiplexerFactory connectionMultiplexerFactory,
            IAggregateSerializer aggregateSerializer,
            IOptionsMonitor<AggregateSnapshotSettings<TState>> aggregateSnapshotSettings)
        {
            if (connectionMultiplexerFactory is null)
                throw new ArgumentNullException(nameof(connectionMultiplexerFactory));
            if (aggregateSerializer is null)
                throw new ArgumentNullException(nameof(aggregateSerializer));
            if (aggregateSnapshotSettings is null)
                throw new ArgumentNullException(nameof(aggregateSnapshotSettings));

            _connectionMultiplexerFactory = connectionMultiplexerFactory;
            _aggregateSerializer = aggregateSerializer;
            _aggregateSnapshotSettings = aggregateSnapshotSettings;
        }

        public async Task SaveAsync(Aggregate<TState> aggregate, CancellationToken cancellationToken = default)
        {
            var connection = _connectionMultiplexerFactory.Create(_aggregateSnapshotSettings.CurrentValue.ConnectionString);
            var db = connection.GetDatabase();

            await db.StringSetAsync(aggregate.Id.ToString(), _aggregateSerializer.Serialize(aggregate));
        }
    }
}
