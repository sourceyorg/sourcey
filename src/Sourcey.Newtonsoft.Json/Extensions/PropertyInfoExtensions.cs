using System.Reflection;

namespace Sourcey.Newtonsoft.Json.Extensions;

internal static class PropertyInfoExtensions
{
    public static bool HasSetter(this PropertyInfo property)
    {
        if (property == null)
            throw new ArgumentNullException(nameof(property));

        return property.GetSetMethod(true) != null;
    }
}
