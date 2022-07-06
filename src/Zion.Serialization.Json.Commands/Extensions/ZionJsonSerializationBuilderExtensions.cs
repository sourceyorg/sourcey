using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Commands.Serialization;
using Zion.Serialization.Json.Builder;

namespace Zion.Serialization.Json.Commands.Extensions
{
    public static class ZionJsonSerializationBuilderExtensions
    {
        public static IZionJsonSerializationBuilder AddCommandSerialization(this IZionJsonSerializationBuilder builder)
        {
            builder.Services.TryAdd(SerializationServices());

            return builder;
        }

        private static IEnumerable<ServiceDescriptor> SerializationServices()
        {
            yield return ServiceDescriptor.Scoped<ICommandDeserializer, JsonCommandDeserializer>();
            yield return ServiceDescriptor.Scoped<ICommandSerializer, JsonCommandSerializer>();
        }
    }
}
