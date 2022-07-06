using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Zion.Serialization.Json.Converters;

namespace Zion.Serialization.Json.Builder
{
    internal readonly struct ZionJsonSerializationBuilder : IZionJsonSerializationBuilder
    {
        public readonly IServiceCollection Services { get; }

        public ZionJsonSerializationBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;

            services.AddScoped<JsonConverter, ActorJsonConverter>();
            services.AddScoped<JsonConverter, CausationJsonConverter>();
            services.AddScoped<JsonConverter, CommandIdJsonConverter>();
            services.AddScoped<JsonConverter, CorrelationJsonConverter>();
            services.AddScoped<JsonConverter, EventIdJsonConverter>();
            services.AddScoped<JsonConverter, NullableActorJsonConverter>();
            services.AddScoped<JsonConverter, NullableCausationJsonConverter>();
            services.AddScoped<JsonConverter, NullableCommandIdJsonConverter>();
            services.AddScoped<JsonConverter, NullableCorrelationJsonConverter>();
            services.AddScoped<JsonConverter, NullableEventIdJsonConverter>();
            services.AddScoped<JsonConverter, NullableQueryIdJsonConverter>();
            services.AddScoped<JsonConverter, NullableSecretJsonConverter>();
            services.AddScoped<JsonConverter, QueryIdJsonConverter>();
            services.AddScoped<JsonConverter, SecretJsonConverter>();
        }
    }
}
