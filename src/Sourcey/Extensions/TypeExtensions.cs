﻿namespace Sourcey.Extensions;

public static class TypeExtensions
{
    public static string FriendlyName(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        if (!type.IsGenericType && !type.IsArray)
            return type.Name;

        if (type.IsArray)
            return type?.GetElementType()?.FriendlyName() + "[]";

        var genericDefinition = type.GetGenericTypeDefinition();

        if (genericDefinition == typeof(Nullable<>))
            return type.GetGenericArguments()[0].FriendlyName() + "?";

        return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(arg => arg.FriendlyName())) + ">";
    }

    public static string FriendlyFullName(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        if (!type.IsGenericType && !type.IsArray)
            return type.FullName ?? type.Name;

        if (type.IsArray)
            return type?.GetElementType()?.FriendlyName() + "[]";

        var genericDefinition = type.GetGenericTypeDefinition();

        if (genericDefinition == typeof(Nullable<>))
            return type.GetGenericArguments()[0].FriendlyName() + "?";

        var name = type.FullName ?? type.Name;
        return name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(arg => arg.FriendlyName())) + ">";
    }

    public static bool IsSubclassOfGeneric(this Type toCheck, Type generic)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
            {
                return true;
            }
            toCheck = toCheck.BaseType;
        }
        return false;
    }
}
