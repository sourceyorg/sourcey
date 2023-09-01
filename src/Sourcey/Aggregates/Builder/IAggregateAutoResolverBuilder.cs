using Sourcey.Events;

namespace Sourcey.Aggregates.Builder;

/// <summary>
/// Interface for building an automatic resolver for aggregates.
/// <typeparam name="TAggregateState">The type of the aggregate state.</typeparam>
/// </summary>
public interface IAggregateAutoResolverBuilder<TAggregateState>
    where TAggregateState : IAggregateState, new()
{
    /// <summary>
    /// Specifies the event type to be resolved.
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <returns>The aggregate auto resolver builder instance.</returns>
    /// </summary>
    IAggregateAutoResolverBuilder<TAggregateState> For<TEvent>()
        where TEvent : IEvent;

    /// <summary>
    /// Specifies the previous and next event types to be resolved.
    /// <typeparam name="TPrevEvent">The type of the previous event.</typeparam>
    /// <typeparam name="TNextEvent">The type of the next event.</typeparam>
    /// <param name="includePermutation">Whether to include permutation.</param>
    /// <returns>The aggregate auto resolver builder instance.</returns>
    /// </summary>
    IAggregateAutoResolverBuilder<TAggregateState> For<TPrevEvent, TNextEvent>(bool includePermutation = false)
        where TPrevEvent : IEvent
        where TNextEvent : IEvent;

    /// <summary>
    /// Specifies the permutation type and event types to be resolved.
    /// <param name="permutation">The permutation type.</param>
    /// <param name="events">The event types.</param>
    /// <returns>The aggregate auto resolver builder instance.</returns>
    /// </summary>
    IAggregateAutoResolverBuilder<TAggregateState> For(PermutationType permutation, params Type[] events);
}
