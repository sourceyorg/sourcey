namespace Sourcey.Serialization.Builder;

public interface ISerializationBuilder
{
    ISerializationBuilder WithEvents();
    ISerializationBuilder WithAggregates();
}
