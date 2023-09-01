using System.Text.Json;
using System.Text.Json.Serialization;
using Sourcey.Keys;

namespace Sourcey.Serialization;

internal sealed class EventIdJsonConverter : JsonConverter<EventId> {
    public override void Write(Utf8JsonWriter writer, EventId value, JsonSerializerOptions options) {
        writer.WriteStringValue(value.ToString());
    }

    public override EventId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        return string.IsNullOrEmpty(value) ? EventId.Unknown : EventId.From(value);
    }
}