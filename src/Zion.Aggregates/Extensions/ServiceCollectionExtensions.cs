using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Aggregates.Stores;
using Zion.Events.Stores;

namespace Zion.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAggregateStore<TEventStoreContext>(this IServiceCollection services)
            where TEventStoreContext : IEventStoreContext
        {
            services.TryAddScoped<IAggregateStore<TEventStoreContext>, AggregateStore<TEventStoreContext>>();
            return services;
        }
    }
}
