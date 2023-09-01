using Newtonsoft.Json;
using Sourcey.Keys;

namespace Sourcey.Newtonsoft.Json.Converters;

public sealed class NullableEventIdJsonConverter : JsonConverter<EventId?>
{
    public override void WriteJson(JsonWriter writer, EventId? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value.ToString());
    }

    public override EventId? ReadJson(JsonReader reader, Type objectType, EventId? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var value = serializer.Deserialize<string>(reader);

        if (value == null)
            return null;

        return EventId.From(value);
    }
}
