using Zion.Core.Builder;
using Zion.Serialization.Json.Builder;

namespace Zion.Serialization.Json.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddJsonSerialization(this IZionBuilder builder, Action<IZionJsonSerializationBuilder> configuration)
        {
            var zionJsonSerializationBuilder = new ZionJsonSerializationBuilder(builder.Services);
            configuration(zionJsonSerializationBuilder);
            return builder;
        }
    }
}
