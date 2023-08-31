using Sourcey.Builder;
using Sourcey.Serialization.Builder;

namespace Sourcey.Extensions;

public static partial class SourceyBuilderExtensions
{
    public static ISourceyBuilder AddSerialization(this ISourceyBuilder builder, Action<ISerializationBuilder> configuration)
    {
        var sourceyJsonSerializationBuilder = new SerializationBuilder(builder.Services);
        configuration(sourceyJsonSerializationBuilder);
        return builder;
    }
}
