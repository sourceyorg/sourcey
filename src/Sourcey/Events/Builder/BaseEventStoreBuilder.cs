using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Sourcey.Aggregates;
using Sourcey.Events.Stores;
using Sourcey.Extensions;
using Sourcey.Projections;
using Sourcey.Projections.Configuration;

namespace Sourcey.Events.Builder;

public abstract class BaseEventStoreBuilder<TEventStoreContext> : IEventStoreBuilder
    where TEventStoreContext : IEventStoreContext
{
    private readonly IServiceCollection _services;


    public BaseEventStoreBuilder(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        _services = services;
    }

    protected abstract TEventStoreContext GetEventStoreContext(IServiceProvider provider);

    public IEventStoreBuilder AddAggregate<TAggregate, TAggregateState>()
        where TAggregate : Aggregate<TAggregateState>
        where TAggregateState : IAggregateState, new()
        => AddAggregates(typeof(TAggregate));

    public IEventStoreBuilder AddAggregates(params Type[] types)
    {
        var aggregateType = typeof(Aggregate<>);
        if(types == null || types.Length == 0)
            return AddAggregatesInternal();

        foreach (var @type in types)
        {
            if (@type == null)
                throw new ArgumentNullException(nameof(types));

            if (!@type.IsSubclassOfGeneric(aggregateType))
                throw new ArgumentException($"Type {@type} is not an aggregate type.");

            _services.AddSingleton(sp => new AggregateEventContextCache(type, () => GetEventStoreContext(sp)));
        }

        return this;
    }

    public IEventStoreBuilder AddAggregatesInternal()
    {
        var eventType = typeof(Aggregate<>);

        var assemblies = DependencyContext.Default?.RuntimeLibraries
            .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
            .Select(Assembly.Load)
            .ToArray();

        var types = assemblies.SelectMany(assembly => assembly.DefinedTypes)
                              .Where(typeInfo => typeInfo.IsClass && !typeInfo.IsAbstract)
                              .Where(eventType.IsAssignableFrom)
                              .Select(typeInfo => typeInfo.AsType())
                              .ToArray();

        if(types == null || types.Length == 0)
            return this;

        return AddAggregates(types);
    }

    public IEventStoreBuilder AddProjections(Action<IStoreProjectorOptions>? action = null)
    {
        var eventType = typeof(IProjection);

        var assemblies = DependencyContext.Default?.RuntimeLibraries
            .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
            .Select(Assembly.Load)
            .ToArray();

        var types = assemblies.SelectMany(assembly => assembly.DefinedTypes)
                              .Where(typeInfo => typeInfo.IsClass && !typeInfo.IsAbstract)
                              .Where(eventType.IsAssignableFrom)
                              .Select(typeInfo => typeInfo.AsType())
                              .ToArray();

        if(types == null || types.Length == 0)
            return this;

        return AddProjections(action, types);
    }

    public IEventStoreBuilder AddProjection<TProjection>()
        where TProjection : class, IProjection
        => AddProjection<TProjection>(action: null);

    public IEventStoreBuilder AddProjection<TProjection>(Action<IStoreProjectorOptions>? action = null)
        where TProjection : class, IProjection
        => AddProjections(action, typeof(TProjection));

    public IEventStoreBuilder AddProjections(params Type[] types)
    {
        if (types == null || types.Length == 0)
            return AddProjections(action: null);

        return AddProjections(action: null, types);
    }


    public IEventStoreBuilder AddProjections(
        Action<IStoreProjectorOptions>? action = null,
        params Type[] types)
    {
        var storeProjectorType = typeof(StoreProjector<>);
        var projectionType = typeof(IProjection);

        foreach (var @type in types)
        {
            if (@type == null)
                throw new ArgumentNullException(nameof(types));

            if (!projectionType.IsAssignableFrom(@type))
                throw new ArgumentException($"Type {@type} is not an aggregate type.");

            _services.AddSingleton(sp => new ProjectionEventContextCache(type, () => GetEventStoreContext(sp)));
            _services.AddSingleton(typeof(IHostedService), storeProjectorType.MakeGenericType(@type));
            var options = new StoreProjectorOptions();
            action?.Invoke((IStoreProjectorOptions)options);
            _services.AddKeyedSingleton(StoreProjectorOptions.GetKey(@type.FriendlyFullName()), options);
        }

        return this;
    }
}
