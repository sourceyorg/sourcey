using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Aggregates.Stores;
using Sourcey.Events.Stores;

namespace Sourcey.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAggregateStore<TEventStoreContext>(this IServiceCollection services)
        where TEventStoreContext : IEventStoreContext
    {
        services.TryAddScoped<IAggregateStore<TEventStoreContext>, AggregateStore<TEventStoreContext>>();
        return services;
    }
}
