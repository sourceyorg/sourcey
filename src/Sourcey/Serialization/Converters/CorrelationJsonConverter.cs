using System.Text.Json;
using System.Text.Json.Serialization;
using Sourcey.Keys;

namespace Sourcey.Serialization;

internal sealed class CorrelationJsonConverter : JsonConverter<Correlation> {
    public override void Write(Utf8JsonWriter writer, Correlation value, JsonSerializerOptions options) {
        writer.WriteStringValue(value.ToString());
    }

    public override Correlation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        return string.IsNullOrEmpty(value) ? Correlation.Unknown : Correlation.From(value);
    }
}