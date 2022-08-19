using System.Text.Json;
using Microsoft.Extensions.Options;
using Zion.Aggregates;
using Zion.Aggregates.Snapshots;

namespace Zion.Redis.Aggregates.Snapshots
{
    internal class AggregateSnapshooter<TState> : IAggregateSnapshooter<TState>
        where TState : IAggregateState, new()
    {
        private readonly IConnectionMultiplexerFactory _connectionMultiplexerFactory;
        private readonly IOptionsMonitor<AggregateSnapshotSettings<TState>> _aggregateSnapshotSettings;

        public AggregateSnapshooter(IConnectionMultiplexerFactory connectionMultiplexerFactory,
            IOptionsMonitor<AggregateSnapshotSettings<TState>> aggregateSnapshotSettings)
        {
            if (connectionMultiplexerFactory is null)
                throw new ArgumentNullException(nameof(connectionMultiplexerFactory));
            if (aggregateSnapshotSettings is null)
                throw new ArgumentNullException(nameof(aggregateSnapshotSettings));

            _connectionMultiplexerFactory = connectionMultiplexerFactory;
            _aggregateSnapshotSettings = aggregateSnapshotSettings;
        }

        public async Task SaveAsync(Aggregate<TState> aggregate, CancellationToken cancellationToken = default)
        {
            var connection = _connectionMultiplexerFactory.Create(_aggregateSnapshotSettings.CurrentValue.ConnectionString);
            var db = connection.GetDatabase();

            await db.StringSetAsync(aggregate.Id.ToString(), JsonSerializer.Serialize(aggregate));
        }
    }
}
