using Zion.Core.Builder;
using Zion.Serialization.Json.Builder;

namespace Zion.Serialization.Json.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionJsonSerializationBuilder AddJsonSerialization(this IZionBuilder builder)
            => new ZionJsonSerializationBuilder(builder.Services);
    }
}
