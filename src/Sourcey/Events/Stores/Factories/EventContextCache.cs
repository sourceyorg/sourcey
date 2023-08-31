namespace Sourcey.Events.Stores;

public record EventContextCache(Type AggregateType, Func<IEventStoreContext> EventContextFactory);