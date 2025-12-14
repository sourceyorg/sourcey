using Newtonsoft.Json;
using Sourcey.Keys;

namespace Sourcey.Newtonsoft.Json.Converters;

public sealed class NullableActorJsonConverter : JsonConverter<Actor?>
{
    public override void WriteJson(JsonWriter writer, Actor? value, JsonSerializer serializer)
    {
        if (!value.HasValue)
        {
            writer.WriteNull();
            return;
        }

        serializer.Serialize(writer, value.Value.ToString());
    }

    public override Actor? ReadJson(JsonReader reader, Type objectType, Actor? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var value = serializer.Deserialize<string>(reader);

        if (value == null)
            return null;

        return Actor.From(value);
    }
}
