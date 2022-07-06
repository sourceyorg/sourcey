using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Core.Builder;
using Zion.Queries.Execution;
using Zion.Queries.Stores;

namespace Zion.Queries.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddQueries(this IZionBuilder builder)
        {
            builder.Services.TryAdd(StoreServices());
            builder.Services.TryAdd(DispatcherServices());

            return builder;
        }

        private static IEnumerable<ServiceDescriptor> StoreServices()
        {
            yield return ServiceDescriptor.Scoped<IQueryStore, NoOpQueryStore>();
        }

        private static IEnumerable<ServiceDescriptor> DispatcherServices()
        {
            yield return ServiceDescriptor.Scoped<IQueryDispatcher, QueryDispatcher>();
        }
    }
}
