using System.Text.Json;
using System.Text.Json.Serialization;
using Sourcey.Keys;

namespace Sourcey.Serialization;

internal sealed class NullableCausationJsonConverter : JsonConverter<Causation?> {
    public override void Write(Utf8JsonWriter writer, Causation? value, JsonSerializerOptions options) {
        writer.WriteStringValue(value?.ToString() ?? string.Empty);
    }

    public override Causation? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        return string.IsNullOrEmpty(value) ? null : Causation.From(value);
    }
}