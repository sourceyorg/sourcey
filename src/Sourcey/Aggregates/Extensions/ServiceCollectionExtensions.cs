using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Aggregates.Stores;
using Sourcey.Events.Stores;

namespace Sourcey.Extensions;

/// <summary>
/// Provides extension methods for registering aggregate store related services in the dependency injection container.
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the aggregate store service to the specified <see cref="IServiceCollection"/>.
    /// <typeparam name="TEventStoreContext">The type of the event store context.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
    /// </summary>
    public static IServiceCollection AddAggregateStore<TEventStoreContext>(this IServiceCollection services)
        where TEventStoreContext : IEventStoreContext
    {
        services.TryAddScoped<IAggregateStore<TEventStoreContext>, AggregateStore<TEventStoreContext>>();
        return services;
    }
}
