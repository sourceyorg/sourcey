using System.Text.Json;
using System.Text.Json.Serialization;
using Sourcey.Keys;

namespace Sourcey.Serialization;

internal sealed class NullableStreamIdJsonConverter : JsonConverter<StreamId?> {
    public override void Write(Utf8JsonWriter writer, StreamId? value, JsonSerializerOptions options) {
        writer.WriteStringValue(value?.ToString() ?? string.Empty);
    }

    public override StreamId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        return string.IsNullOrEmpty(value) ? null : StreamId.From(value);
    }
}