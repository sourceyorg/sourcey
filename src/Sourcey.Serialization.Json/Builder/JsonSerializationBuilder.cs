using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Sourcey.Aggregates.Serialization;
using Sourcey.Events.Serialization;
using Sourcey.Serialization.Json.Aggregates;
using Sourcey.Serialization.Json.Converters;
using Sourcey.Serialization.Json.Events;

namespace Sourcey.Serialization.Json.Builder;

internal readonly struct JsonSerializationBuilder : IJsonSerializationBuilder
{
    public readonly IServiceCollection _services;

    public JsonSerializationBuilder(IServiceCollection services)
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

    public IJsonSerializationBuilder AddEventSerialization()
    {
        _services.TryAddSingleton<IEventSerializer, EventSerializer>();
        _services.TryAddSingleton<IEventDeserializer, EventDeserializer>();
        return this;
    }
    

    public IJsonSerializationBuilder AddAggregateSerialization()
    {
        _services.TryAddSingleton<IAggregateSerializer, AggregateSerializer>();
        _services.TryAddSingleton<IAggregateDeserializer, AggregateDeserializer>();
        return this;
    }
}
