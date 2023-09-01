using System.Text.Json;
using System.Text.Json.Serialization;
using Sourcey.Keys;

namespace Sourcey.Serialization;

internal sealed class CausationJsonConverter : JsonConverter<Causation> {
    public override void Write(Utf8JsonWriter writer, Causation value, JsonSerializerOptions options) {
        writer.WriteStringValue(value.ToString());
    }

    public override Causation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        return string.IsNullOrEmpty(value) ? Causation.Unknown : Causation.From(value);
    }
}