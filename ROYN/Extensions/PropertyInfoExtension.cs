using System;
using System.Reflection;

namespace ROYN.Extensions
{
    internal static class PropertyInfoExtension
    {
        internal static bool IsAttributeDefined<TAttribute>(this PropertyInfo propertyInfo) where TAttribute : Attribute
        {
            return propertyInfo.GetCustomAttribute<TAttribute>() != null;
        }
    }
}