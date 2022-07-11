﻿namespace Zion.Core.Extensions
{
    public static class TypeExtensions
    {
        public static string? FriendlyName(this Type type)
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

        public static string? FriendlyFullName(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!type.IsGenericType && !type.IsArray)
                return type.FullName;

            if (type.IsArray)
                return type?.GetElementType()?.FriendlyName() + "[]";

            var genericDefinition = type.GetGenericTypeDefinition();

            if (genericDefinition == typeof(Nullable<>))
                return type.GetGenericArguments()[0].FriendlyName() + "?";

            var name = type.FullName ?? type.Name;
            return name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(arg => arg.FriendlyName())) + ">";
        }
    }
}
