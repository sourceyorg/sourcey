namespace Sourcey.Events.Stores;

public record ProjectionEventContextCache(Type AggregateType, Func<IEventStoreContext> EventContextFactory)
    : EventContextCache(AggregateType, EventContextFactory);