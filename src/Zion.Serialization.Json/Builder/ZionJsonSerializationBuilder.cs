using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Zion.Aggregates.Serialization;
using Zion.Commands.Serialization;
using Zion.Events.Serialization;
using Zion.Queries.Serialization;
using Zion.Serialization.Json.Aggregates;
using Zion.Serialization.Json.Commands;
using Zion.Serialization.Json.Converters;
using Zion.Serialization.Json.Events;
using Zion.Serialization.Json.Queries;

namespace Zion.Serialization.Json.Builder
{
    internal readonly struct ZionJsonSerializationBuilder : IZionJsonSerializationBuilder
    {
        public readonly IServiceCollection _services;

        public ZionJsonSerializationBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            _services = services;

            services.AddSingleton<JsonConverter, ActorJsonConverter>();
            services.AddSingleton<JsonConverter, CausationJsonConverter>();
            services.AddSingleton<JsonConverter, CommandIdJsonConverter>();
            services.AddSingleton<JsonConverter, CorrelationJsonConverter>();
            services.AddSingleton<JsonConverter, EventIdJsonConverter>();
            services.AddSingleton<JsonConverter, QueryIdJsonConverter>();
            services.AddSingleton<JsonConverter, SecretJsonConverter>();
            services.AddSingleton<JsonConverter, StreamIdJsonConverter>();
            services.AddSingleton<JsonConverter, NullableActorJsonConverter>();
            services.AddSingleton<JsonConverter, NullableCausationJsonConverter>();
            services.AddSingleton<JsonConverter, NullableCommandIdJsonConverter>();
            services.AddSingleton<JsonConverter, NullableCorrelationJsonConverter>();
            services.AddSingleton<JsonConverter, NullableEventIdJsonConverter>();
            services.AddSingleton<JsonConverter, NullableQueryIdJsonConverter>();
            services.AddSingleton<JsonConverter, NullableSecretJsonConverter>();
            services.AddSingleton<JsonConverter, NullableStreamIdJsonConverter>();
        }

        public IZionJsonSerializationBuilder AddCommandSerialization()
        {
            _services.TryAddSingleton<ICommandSerializer, CommandSerializer>();
            _services.TryAddSingleton<ICommandDeserializer, CommandDeserializer>();
            return this;
        }

        public IZionJsonSerializationBuilder AddEventSerialization()
        {
            _services.TryAddSingleton<IEventSerializer, EventSerializer>();
            _services.TryAddSingleton<IEventDeserializer, EventDeserializer>();
            _services.TryAddSingleton<IEventNotificationSerializer, EventNotificationSerializer>();
            _services.TryAddSingleton<IEventNotificationDeserializer, EventNotificationDeserializer>();
            return this;
        }

        public IZionJsonSerializationBuilder AddQuerySerialization()
        {
            _services.TryAddSingleton<IQuerySerializer, QuerySerializer>();
            _services.TryAddSingleton<IQueryDeserializer, QueryDeserializer>();
            return this;
        }

        public IZionJsonSerializationBuilder AddAggregateSerialization()
        {
            _services.TryAddSingleton<IAggregateSerializer, AggregateSerializer>();
            _services.TryAddSingleton<IAggregateDeserializer, AggregateDeserializer>();
            return this;
        }
    }
}
