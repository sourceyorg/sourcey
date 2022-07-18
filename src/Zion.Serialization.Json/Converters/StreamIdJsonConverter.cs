﻿using Newtonsoft.Json;
using Zion.Events.Streams;

namespace Zion.Serialization.Json.Converters
{
    public sealed class StreamIdJsonConverter : JsonConverter<StreamId>
    {
        public override void WriteJson(JsonWriter writer, StreamId value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override StreamId ReadJson(JsonReader reader, Type objectType, StreamId existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<string>(reader);
            return StreamId.From(value);
        }
    }
}
