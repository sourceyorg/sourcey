using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Zion.Queries.Serialization;

namespace Zion.Serialization.Json.Queries
{
    internal sealed class QuerySerializer : IQuerySerializer
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public QuerySerializer(IEnumerable<JsonConverter> jsonConverters)
        {
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None,
                Converters = jsonConverters?.ToList() ?? new List<JsonConverter>()
            };
        }
        public string Serialize<T>(T data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return JsonConvert.SerializeObject(data, _serializerSettings);
        }
    }
}
