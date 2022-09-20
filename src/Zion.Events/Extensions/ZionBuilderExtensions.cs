using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Zion.Core.Builder;
using Zion.Events;
using Zion.Events.Cache;
using Zion.Events.Execution;
using Zion.Events.Streams;

namespace Zion.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddEvents(this IZionBuilder builder)
        {
            builder.Services.TryAdd(GetStreamServices());
            builder.Services.TryAdd(GetCacheServices());
            builder.Services.TryAdd(GetDispatcherServices());

            return builder;
        }

        public static IZionBuilder RegisterEventCache<TEvent>(this IZionBuilder builder)
            where TEvent : IEvent
        {
            builder.AddEvents();
            builder.Services.AddSingleton(new EventTypeCacheRecord(typeof(TEvent)));
            return builder;
        }

        public static IZionBuilder RegisterEventCache(this IZionBuilder builder, params Type[] types)
        {
            builder.AddEvents();
            foreach (var type in types)
                builder.Services.AddSingleton(new EventTypeCacheRecord(type));

            return builder;
        }

        public static IZionBuilder RegisterEventCache(this IZionBuilder builder)
        {
            builder.AddEvents();
            var eventType = typeof(IEvent);

            var assemblies = DependencyContext.Default.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
                .Select(Assembly.Load)
                .ToArray();

            var types = assemblies.SelectMany(assembly => assembly.DefinedTypes)
                                  .Where(typeInfo => typeInfo.IsClass && !typeInfo.IsAbstract)
                                  .Where(typeInfo => eventType.IsAssignableFrom(typeInfo))
                                  .Select(typeInfo => typeInfo.AsType())
                                  .ToArray();

            return builder.RegisterEventCache(types);
        }

        private static IEnumerable<ServiceDescriptor> GetStreamServices()
        {
            yield return ServiceDescriptor.Scoped<IEventStreamManager, EventStreamManager>();
        }

        private static IEnumerable<ServiceDescriptor> GetCacheServices()
        {
            yield return ServiceDescriptor.Singleton<IEventTypeCache, EventTypeCache>();
        }

        private static IEnumerable<ServiceDescriptor> GetDispatcherServices()
        {
            yield return ServiceDescriptor.Scoped<IEventDispatcher, EventDispatcher>();
        }
    }
}
