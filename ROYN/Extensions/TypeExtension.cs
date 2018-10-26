using System;
using System.Linq;
using System.Reflection;

namespace ROYN.Extensions
{
    internal static class TypeExtension
    {
        internal static bool IsAttributeDefined<TAttribute>(this Type type) where TAttribute : Attribute
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetCustomAttributes().Any(x => x is TAttribute);
        }
    }
}