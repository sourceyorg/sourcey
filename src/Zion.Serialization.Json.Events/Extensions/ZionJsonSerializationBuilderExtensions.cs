using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Events.Serialization;
using Zion.Serialization.Json.Builder;

namespace Zion.Serialization.Json.Events.Extensions
{
    public static class ZionJsonSerializationBuilderExtensions
    {
        public static IZionJsonSerializationBuilder AddEventSerialization(this IZionJsonSerializationBuilder builder)
        {
            builder.Services.TryAdd(SerializationServices());

            return builder;
        }

        private static IEnumerable<ServiceDescriptor> SerializationServices()
        {
            yield return ServiceDescriptor.Scoped<IEventDeserializer, JsonEventDeserializer>();
            yield return ServiceDescriptor.Scoped<IEventSerializer, JsonEventSerializer>();
        }
    }
}
