using System.Text.Json;
using System.Text.Json.Serialization;
using Sourcey.Aggregates.Serialization;

namespace Sourcey.Serialization.Aggregates;

internal sealed class AggregateDeserializer : IAggregateDeserializer
{
    private readonly JsonSerializerOptions _serializerOptions;

    public AggregateDeserializer(IEnumerable<JsonConverter> jsonConverters)
    {
        jsonConverters ??= Enumerable.Empty<JsonConverter>();

        _serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        foreach (var converter in jsonConverters)
            _serializerOptions.Converters.Add(converter);
    }

    public object Deserialize(string data, Type type)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        return JsonSerializer.Deserialize(data, type, _serializerOptions);
    }

    public T Deserialize<T>(string data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        return JsonSerializer.Deserialize<T>(data, _serializerOptions);
    }
}