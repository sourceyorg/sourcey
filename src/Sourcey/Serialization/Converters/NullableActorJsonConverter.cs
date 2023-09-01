using System.Text.Json;
using System.Text.Json.Serialization;
using Sourcey.Keys;

namespace Sourcey.Serialization;

internal sealed class NullableActorJsonConverter : JsonConverter<Actor?> {
    public override void Write(Utf8JsonWriter writer, Actor? value, JsonSerializerOptions options) {
        writer.WriteStringValue(value?.ToString() ?? string.Empty);
    }

    public override Actor? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        return string.IsNullOrEmpty(value) ? null : Actor.From(value);
    }
}