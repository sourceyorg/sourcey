﻿using Newtonsoft.Json;
using Sourcey.Core.Keys;

namespace Sourcey.Serialization.Json.Converters;

public sealed class ActorJsonConverter : JsonConverter<Actor>
{
    public override void WriteJson(JsonWriter writer, Actor value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value.ToString());
    }

    public override Actor ReadJson(JsonReader reader, Type objectType, Actor existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var value = serializer.Deserialize<string>(reader);
        return Actor.From(value);
    }
}
