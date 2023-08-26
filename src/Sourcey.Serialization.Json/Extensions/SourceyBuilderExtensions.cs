using Sourcey.Core.Builder;
using Sourcey.Serialization.Json.Builder;

namespace Sourcey.Extensions;

public static class SourceyBuilderExtensions
{
    public static ISourceyBuilder AddJsonSerialization(this ISourceyBuilder builder, Action<IJsonSerializationBuilder> configuration)
    {
        var sourceyJsonSerializationBuilder = new JsonSerializationBuilder(builder.Services);
        configuration(sourceyJsonSerializationBuilder);
        return builder;
    }
}
