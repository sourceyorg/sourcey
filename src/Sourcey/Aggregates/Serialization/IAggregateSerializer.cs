namespace Sourcey.Aggregates.Serialization;

/// <summary>
/// Interface for serializing aggregate data.
/// </summary>
public interface IAggregateSerializer
{
    /// <summary>
    /// Serializes the specified data.
    /// <typeparam name="T">The type of data to serialize.</typeparam>
    /// <param name="data">The data to serialize.</param>
    /// <returns>A string representation of the serialized data.</returns>
    /// </summary>
    string Serialize<T>(T data);
}
