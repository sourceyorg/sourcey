using Sourcey.Builder;
using Sourcey.Newtonsoft.Json.Builder;
using Sourcey.Serialization.Builder;

namespace Sourcey.Extensions;

public static class SourceyBuilderExtensions
{
    public static ISourceyBuilder AddNewtonsoftJsonSerialization(this ISourceyBuilder builder, Action<ISerializationBuilder> configuration)
    {
        var serializationBuilder = new SerializationBuilder(builder.Services);
        configuration(serializationBuilder);
        return builder;
    }
}
