using Newtonsoft.Json;
using Sourcey.Queries.Serialization;
using Sourcey.Serialization.Json.Resolvers;

namespace Sourcey.Serialization.Json.Queries
{
    internal sealed class QueryDeserializer : IQueryDeserializer
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public QueryDeserializer(IEnumerable<JsonConverter> jsonConverters)
        {
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new ImmutablePropertyCamelCasePropertyNamesContactResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                Converters = jsonConverters?.ToList() ?? new List<JsonConverter>()
            };
        }

        public object Deserialize(string data, Type type)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return JsonConvert.DeserializeObject(data, type, _serializerSettings);
        }
        public T Deserialize<T>(string data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return JsonConvert.DeserializeObject<T>(data, _serializerSettings);
        }
    }
}
