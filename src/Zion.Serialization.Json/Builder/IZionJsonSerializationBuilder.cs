using Microsoft.Extensions.DependencyInjection;

namespace Zion.Serialization.Json.Builder
{
    public interface IZionJsonSerializationBuilder
    {
        IZionJsonSerializationBuilder AddCommandSerialization();
        IZionJsonSerializationBuilder AddQuerySerialization();
        IZionJsonSerializationBuilder AddEventSerialization();
        IZionJsonSerializationBuilder AddAggregateSerialization();
    }
}
