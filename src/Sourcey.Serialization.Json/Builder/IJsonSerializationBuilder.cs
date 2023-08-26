namespace Sourcey.Serialization.Json.Builder;

public interface IJsonSerializationBuilder
{
    IJsonSerializationBuilder AddEventSerialization();
    IJsonSerializationBuilder AddAggregateSerialization();
}
