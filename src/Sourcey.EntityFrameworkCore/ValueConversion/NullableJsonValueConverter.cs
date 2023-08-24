using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sourcey.EntityFrameworkCore.ValueConversion
{
    internal sealed class NullableJsonValueConverter<T> : ValueConverter<T?, string?>
        where T : class
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public NullableJsonValueConverter(ConverterMappingHints mappingHints = default)
            : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
        {
        }

        private static string? ConvertTo(T? model)
            => model is not null ? JsonConvert.SerializeObject(model, _settings) : string.Empty;

        private static T? ConvertFrom(string? json)
            => string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<T>(json, _settings);
    }
}
