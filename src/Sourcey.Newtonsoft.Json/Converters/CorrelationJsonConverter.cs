using Newtonsoft.Json;
using Sourcey.Keys;

namespace Sourcey.Newtonsoft.Json.Converters;

public sealed class CorrelationJsonConverter : JsonConverter<Correlation>
{
    public override void WriteJson(JsonWriter writer, Correlation value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value.ToString());
    }

    public override Correlation ReadJson(JsonReader reader, Type objectType, Correlation existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var value = serializer.Deserialize<string>(reader);
        return Correlation.From(value);
    }
}
