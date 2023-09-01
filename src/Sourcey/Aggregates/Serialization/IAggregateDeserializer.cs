namespace Sourcey.Aggregates.Serialization
{
    /// <summary>
    /// Defines the interface for an aggregate deserializer.
    /// </summary>
    public interface IAggregateDeserializer
    {
        /// <summary>
        /// Deserializes the specified data into an object of the specified type.
        /// <param name="data">The data to deserialize.</param>
        /// <param name="type">The type of object to deserialize the data into.</param>
        /// <returns>The deserialized object.</returns>
        /// </summary>
        object Deserialize(string data, Type type);

        /// <summary>
        /// Deserializes the specified data into an object of type T.
        /// <typeparam name="T">The type of object to deserialize the data into.</typeparam>
        /// <param name="data">The data to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        /// </summary>
        T Deserialize<T>(string data);
    }
}
