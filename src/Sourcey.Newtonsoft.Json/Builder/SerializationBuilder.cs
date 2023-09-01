using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Sourcey.Aggregates.Serialization;
using Sourcey.Events.Serialization;
using Sourcey.Newtonsoft.Json.Aggregates;
using Sourcey.Newtonsoft.Json.Converters;
using Sourcey.Newtonsoft.Json.Events;
using Sourcey.Serialization.Builder;

namespace Sourcey.Newtonsoft.Json.Builder;

internal readonly struct SerializationBuilder : ISerializationBuilder
{
    private readonly IServiceCollection _services;

    public SerializationBuilder(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        _services = services;

        services.AddSingleton<JsonConverter, ActorJsonConverter>();
        services.AddSingleton<JsonConverter, CausationJsonConverter>();
        services.AddSingleton<JsonConverter, CorrelationJsonConverter>();
        services.AddSingleton<JsonConverter, EventIdJsonConverter>();
        services.AddSingleton<JsonConverter, StreamIdJsonConverter>();
        services.AddSingleton<JsonConverter, NullableActorJsonConverter>();
        services.AddSingleton<JsonConverter, NullableCausationJsonConverter>();
        services.AddSingleton<JsonConverter, NullableCorrelationJsonConverter>();
        services.AddSingleton<JsonConverter, NullableEventIdJsonConverter>();
        services.AddSingleton<JsonConverter, NullableStreamIdJsonConverter>();
    }

    public ISerializationBuilder WithEvents()
    {
        _services.TryAddSingleton<IEventSerializer, EventSerializer>();
        _services.TryAddSingleton<IEventDeserializer, EventDeserializer>();
        return this;
    }
    

    public ISerializationBuilder WithAggregates()
    {
        _services.TryAddSingleton<IAggregateSerializer, AggregateSerializer>();
        _services.TryAddSingleton<IAggregateDeserializer, AggregateDeserializer>();
        return this;
    }
}
