using Microsoft.Extensions.DependencyInjection;

namespace Sourcey.Serialization.Json.Builder
{
    public interface IJsonSerializationBuilder
    {
        IJsonSerializationBuilder AddCommandSerialization();
        IJsonSerializationBuilder AddQuerySerialization();
        IJsonSerializationBuilder AddEventSerialization();
        IJsonSerializationBuilder AddAggregateSerialization();
    }
}
