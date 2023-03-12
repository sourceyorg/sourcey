using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Zion.Serialization.Json.ValueProviders
{
    public sealed class ImmutablePropertyValueProvider : IValueProvider
    {
        private readonly MemberInfo _member;
        private readonly FieldInfo _compilerField;

        public ImmutablePropertyValueProvider(MemberInfo member, FieldInfo compilerField)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            if (!(member is PropertyInfo) && !(member is FieldInfo))
                throw new ArgumentException($"MemberInfo '{member.Name}' must be of type {nameof(FieldInfo)} or {nameof(PropertyInfo)}");

            _member = member;
            _compilerField = compilerField;
        }

        public object GetValue(object target)
        {
            // https://github.com/dotnet/corefx/issues/26053
            if (_member is PropertyInfo propertyInfo && propertyInfo.PropertyType.IsByRef)
                throw new InvalidOperationException($"Could not create getter for '{propertyInfo.Name}'. ByRef return values are not supported.");

            switch (_member)
            {
                case PropertyInfo prop:
                    return prop.GetValue(target);
                case FieldInfo field:
                    return field.GetValue(target);
            }

            throw new ArgumentException($"MemberInfo '{_member.Name}' must be of type '{nameof(FieldInfo)}' or '{nameof(PropertyInfo)}'");
        }
        public void SetValue(object target, object value)
        {
            if (_compilerField == null)
                throw new InvalidOperationException($"Cannot set value for '{_member.Name}' for '{target.GetType()}'. Compiler generated backing field '<{_member.Name}>k__BackingField' is missing.");

            _compilerField.SetValue(target, value);
        }
    }
}
