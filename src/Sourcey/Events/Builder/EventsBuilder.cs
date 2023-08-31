using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Sourcey.Events.Cache;

namespace Sourcey.Events.Builder;

internal readonly struct EventsBuilder: IEventsBuilder
{
    public readonly IServiceCollection Services { get; }

    public EventsBuilder(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        Services = services;
    }

    public IEventsBuilder RegisterEventCache<TEvent>()
        where TEvent : IEvent
    {
        Services.AddSingleton(new EventTypeCacheRecord(typeof(TEvent)));
        return this;
    }

    public IEventsBuilder RegisterEventCache(params Type[] types)
    {
        var eventType = typeof(IEvent);
        foreach (var type in types)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(types));
                
            if(!eventType.IsAssignableFrom(type))
                throw new ArgumentException($"Type {type} is not an event type.");

            Services.AddSingleton(new EventTypeCacheRecord(type));
        }

        return this;
    }

    public IEventsBuilder RegisterEventCache()
    {
        var eventType = typeof(IEvent);

        var assemblies = DependencyContext.Default?.RuntimeLibraries
            .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
            .Select(Assembly.Load)
            .ToArray();

        var types = assemblies.SelectMany(assembly => assembly.DefinedTypes)
                              .Where(typeInfo => typeInfo.IsClass && !typeInfo.IsAbstract)
                              .Where(typeInfo => eventType.IsAssignableFrom(typeInfo))
                              .Select(typeInfo => typeInfo.AsType())
                              .ToArray();

        return RegisterEventCache(types);
    }
}