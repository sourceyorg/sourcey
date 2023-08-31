using System.Text.Json;
using System.Text.Json.Serialization;
using Sourcey.Events.Serialization;

namespace Sourcey.Serialization.Events;

internal sealed class EventSerializer : IEventSerializer
{
    private readonly JsonSerializerOptions _serializerOptions;

    public EventSerializer(IEnumerable<JsonConverter> jsonConverters)
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

        return JsonSerializer.Serialize(data, _serializerOptions);
    }
}