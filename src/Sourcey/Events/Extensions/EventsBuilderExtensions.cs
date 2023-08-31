using Sourcey.Events.Builder;
using Sourcey.Events.Stores.InMemory;

namespace Sourcey.Extensions;

public static class EventBuilderExtensions
{
    public static IEventsBuilder WithInMemoryStore(this IEventsBuilder builder, Action<IEventStoreBuilder> action)
    {
        action(new InMemoryStoreBuilder(builder.Services));
        return builder;
    }
}