using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Commands.Execution;
using Zion.Commands.Stores;
using Zion.Core.Builder;

namespace Zion.Commands.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddCommands(this IZionBuilder builder)
        {
            builder.Services.TryAdd(StoreServices());
            builder.Services.TryAdd(DispatcherServices());

            return builder;
        }

        private static IEnumerable<ServiceDescriptor> StoreServices()
        {
            yield return ServiceDescriptor.Scoped<ICommandStore, NoOpCommandStore>();
        }

        private static IEnumerable<ServiceDescriptor> DispatcherServices()
        {
            yield return ServiceDescriptor.Scoped<ICommandDispatcher, CommandDispatcher>();
        }
    }
}
