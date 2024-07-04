using Microsoft.Extensions.DependencyInjection;
using Sourcey.Events;

namespace Sourcey.Aggregates;

/// <summary>
/// <inheritdoc/>
/// </summary>
internal sealed class AggregateFactory(IServiceProvider serviceProvider) : IAggregateFactory
{
    private readonly IServiceProvider _serviceProvider =
        serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TAggregate FromHistory<TAggregate, TState>(IEnumerable<IEvent>? events = null)
        where TAggregate : Aggregate<TState>
        where TState : IAggregateState, new()
    {
        var aggregate = _serviceProvider.GetRequiredService<TAggregate>();

        aggregate.FromHistory(events ?? []);

        return aggregate;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TAggregate Create<TAggregate, TState>()
        where TAggregate : Aggregate<TState>
        where TState : IAggregateState, new()
        => FromHistory<TAggregate, TState>(null);
}
