using Newtonsoft.Json;
using Sourcey.Keys;

namespace Sourcey.Newtonsoft.Json.Converters;

public sealed class NullableStreamIdJsonConverter : JsonConverter<StreamId?>
{
    public override void WriteJson(JsonWriter writer, StreamId? value, JsonSerializer serializer)
    {
        if (!value.HasValue)
        {
            writer.WriteNull();
            return;
        }

        serializer.Serialize(writer, value.Value.ToString());
    }

    public override StreamId? ReadJson(JsonReader reader, Type objectType, StreamId? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var value = serializer.Deserialize<string>(reader);

        if (value == null)
            return null;

        return StreamId.From(value);
    }
}
