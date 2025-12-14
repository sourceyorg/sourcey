using Microsoft.Extensions.DependencyInjection;
using Sourcey.Events;

namespace Sourcey.Aggregates.Concurrency;

internal sealed class ConflictResolver : IConflictResolver
{
    private readonly IServiceProvider _serviceProvider;

    public ConflictResolver(IServiceProvider serviceProvider)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        _serviceProvider = serviceProvider;
    }

    public async Task<ConflictAction> ResolveAsync<TAggregateState, TPrevEvent, TNextEvent, TConflictingEvent>(Aggregate<TAggregateState> aggregate, TPrevEvent? prevEvent, TNextEvent? nextEvent, TConflictingEvent? conflictingEvent)
        where TAggregateState : IAggregateState, new()
        where TPrevEvent : IEvent
        where TNextEvent : IEvent
        where TConflictingEvent : IEvent
    {
        if (prevEvent is null && nextEvent is null && conflictingEvent is null)
        {
            var resolution = _serviceProvider.GetService<IConflictResolution<TAggregateState>>();

            if (resolution == null)
                return ConflictAction.Throw;

            return await resolution.ResolveAsync(aggregate).ConfigureAwait(false);
        }

        if (conflictingEvent is not null)
        {
            var resolution = _serviceProvider.GetService<IConflictResolution<TAggregateState, TConflictingEvent>>();

            if (resolution == null)
                return ConflictAction.Throw;

            return await resolution.ResolveAsync(aggregate, conflictingEvent).ConfigureAwait(false);
        }

        if (prevEvent is not null || nextEvent is not null)
        {
            var resolution = _serviceProvider.GetService<IConflictResolution<TAggregateState, TPrevEvent, TNextEvent>>();

            if (resolution == null)
                return ConflictAction.Throw;

            return await resolution.ResolveAsync(aggregate, prevEvent, nextEvent).ConfigureAwait(false);
        }

        return ConflictAction.Throw;
    }
}
