using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Events.Builder;

namespace Sourcey.Events.Stores.InMemory;

internal sealed class InMemoryStoreBuilder : BaseEventStoreBuilder<InMemoryContext>
{
    public InMemoryStoreBuilder(IServiceCollection services) : base(services)
    {
        services.TryAddSingleton<IEventContextFactory, EventContextFactory>();
        services.TryAddSingleton<IEventModelFactory, EventModelFactory>();
        services.TryAddSingleton<InMemoryStore>();
        services.TryAddScoped<IEventStore<InMemoryContext>, InMemoryEventStore>();
    }

    protected override InMemoryContext GetEventStoreContext(IServiceProvider provider)
        => new InMemoryContext();
}