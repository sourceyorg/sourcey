using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Zion.EntityFrameworkCore.ChangeTracking
{
    internal sealed class NullableJsonValueComparer<T> : ValueComparer<T?>
    {
        private static readonly JsonSerializerSettings _settings = new()
        {
            TypeNameHandling = TypeNameHandling.None,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public NullableJsonValueComparer()
            : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

        private static bool IsEqual(T? left, T? right)
        {
            if (left is null || right is null)
                return false;

            if (left is IEquatable<T> equatable)
                return equatable.Equals(right);

            return ConvertTo(left)?.Equals(ConvertTo(right)) == true;
        }
        private static int HashCode(T? model)
        {
            if (model == null)
                return 0;

            if (model is IEquatable<T>)
                return model.GetHashCode();

            return ConvertTo(model)?.GetHashCode() ?? 0; 
        }
        private static T? CreateSnapshot(T? model)
        {
            if (model is ICloneable cloneable)
                return (T)cloneable.Clone();

            return ConvertFrom(ConvertTo(model));
        }
        private static string? ConvertTo(T? model)
        {
            if (model == null)
                return null;

            return JsonConvert.SerializeObject(model, _settings);
        }
        private static T? ConvertFrom(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonConvert.DeserializeObject<T>(json, _settings);
        }
    }
}
