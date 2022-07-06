using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Queries.Serialization;
using Zion.Serialization.Json.Builder;

namespace Zion.Serialization.Json.Queries.Extensions
{
    public static class ZionJsonSerializationBuilderExtensions
    {
        public static IZionJsonSerializationBuilder AddQuerySerialization(this IZionJsonSerializationBuilder builder)
        {
            builder.Services.TryAdd(SerializationServices());

            return builder;
        }

        private static IEnumerable<ServiceDescriptor> SerializationServices()
        {
            yield return ServiceDescriptor.Scoped<IQueryDeserializer, JsonQueryDeserializer>();
            yield return ServiceDescriptor.Scoped<IQuerySerializer, JsonQuerySerializer>();
        }
    }
}
