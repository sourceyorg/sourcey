using System.Text.Json;
using System.Text.Json.Serialization;
using Sourcey.Keys;

namespace Sourcey.Serialization;

internal sealed class NullableCorrelationJsonConverter : JsonConverter<Correlation?> {
    public override void Write(Utf8JsonWriter writer, Correlation? value, JsonSerializerOptions options) {
        writer.WriteStringValue(value?.ToString() ?? string.Empty);
    }

    public override Correlation? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        return string.IsNullOrEmpty(value) ? null : Correlation.From(value);
    }
}