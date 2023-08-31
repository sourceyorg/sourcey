namespace Sourcey.Serialization.Builder;

public interface ISerializationBuilder
{
    ISerializationBuilder AddEventSerialization();
    ISerializationBuilder AddAggregateSerialization();
}
