using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Sourcey.Builder;
using Sourcey.Events;
using Sourcey.Events.Builder;
using Sourcey.Events.Cache;
using Sourcey.Events.Streams;

namespace Sourcey.Extensions;

public static partial class SourceyBuilderExtensions
{
    [Obsolete("Use AddEvents(Action<IEventsBuilder> options) instead")]
    public static ISourceyBuilder AddEvents(this ISourceyBuilder builder)
    {
        builder.Services.TryAddScoped<IEventStreamManager, EventStreamManager>();
        builder.Services.TryAddScoped<IEventTypeCache, EventTypeCache>();

        return builder;
    }

    public static ISourceyBuilder AddEvents(this ISourceyBuilder builder, Action<IEventsBuilder> options)
    {
        builder.Services.TryAddScoped<IEventStreamManager, EventStreamManager>();
        builder.Services.TryAddScoped<IEventTypeCache, EventTypeCache>();
        options(new EventsBuilder(builder.Services));

        return builder;
    }

    [Obsolete("Use IEventsBuilder.RegisterEventCache instead")]
    public static ISourceyBuilder RegisterEventCache<TEvent>(this ISourceyBuilder builder)
        where TEvent : IEvent
    {
        builder.AddEvents();
        builder.Services.AddSingleton(new EventTypeCacheRecord(typeof(TEvent)));
        return builder;
    }

    [Obsolete("Use IEventsBuilder.RegisterEventCache instead")]
    public static ISourceyBuilder RegisterEventCache(this ISourceyBuilder builder, params Type[] types)
    {
        builder.AddEvents();
        foreach (var type in types)
            builder.Services.AddSingleton(new EventTypeCacheRecord(type));

        return builder;
    }


    [Obsolete("Use IEventsBuilder.RegisterEventCache instead")]
    public static ISourceyBuilder RegisterEventCache(this ISourceyBuilder builder)
    {
        builder.AddEvents();
        var eventType = typeof(IEvent);

        var assemblies = DependencyContext.Default?.RuntimeLibraries
            .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
            .Select(Assembly.Load)
            .ToArray();

        var types = assemblies.SelectMany(assembly => assembly.DefinedTypes)
                              .Where(typeInfo => typeInfo.IsClass && !typeInfo.IsAbstract)
                              .Where(eventType.IsAssignableFrom)
                              .Select(typeInfo => typeInfo.AsType())
                              .ToArray();

        return builder.RegisterEventCache(types);
    }
}
