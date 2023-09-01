using System.Text.Json;
using System.Text.Json.Serialization;
using Sourcey.Aggregates.Serialization;

namespace Sourcey.Serialization.Aggregates;

internal sealed class AggregateSerializer : IAggregateSerializer
{
    private readonly JsonSerializerOptions _serializerOptions;

    public AggregateSerializer(IEnumerable<JsonConverter> jsonConverters)
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
    
    public string Serialize<T>(T data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        return JsonSerializer.Serialize<object>(data, _serializerOptions);
    }
}