using Newtonsoft.Json;
using Sourcey.Keys;

namespace Sourcey.Serialization.Json.Converters;

public sealed class CausationJsonConverter : JsonConverter<Causation>
{
    public override void WriteJson(JsonWriter writer, Causation value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value.ToString());
    }

    public override Causation ReadJson(JsonReader reader, Type objectType, Causation existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var value = serializer.Deserialize<string>(reader);
        return Causation.From(value);
    }
}
