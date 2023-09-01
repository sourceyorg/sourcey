namespace Sourcey.Events.Stores;

public record AggregateEventContextCache(Type AggregateType, Func<IEventStoreContext> EventContextFactory)
    :EventContextCache(AggregateType, EventContextFactory);