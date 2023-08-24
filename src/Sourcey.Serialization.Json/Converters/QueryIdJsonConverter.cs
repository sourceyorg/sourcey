using Newtonsoft.Json;
using Sourcey.Queries;

namespace Sourcey.Serialization.Json.Converters
{
    public sealed class QueryIdJsonConverter : JsonConverter<QueryId>
    {
        public override void WriteJson(JsonWriter writer, QueryId value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override QueryId ReadJson(JsonReader reader, Type objectType, QueryId existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<string>(reader);
            return QueryId.From(value);
        }
    }
}
