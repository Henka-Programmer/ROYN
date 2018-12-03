using System;
using System.Collections.Generic;
using System.Reflection;

namespace ROYN
{
    public static class Royn
    {
        internal static readonly Dictionary<Type, Type> _map = new Dictionary<Type, Type>();
        internal static readonly Dictionary<Type, PropertyInfo[]> _typesProperties = new Dictionary<Type, PropertyInfo[]>();

        public static List<Type> FlattTypesMap(Type type, List<Type> visited = null)
        {
            visited = visited ?? new List<Type>();
            visited.Add(type);
            foreach (var p in GetPropertyInfos(type))
            {
                if (p.PropertyType.IsClass && p.PropertyType != typeof(string) && !visited.Contains(p.PropertyType))
                {
                    FlattTypesMap(p.PropertyType, visited);
                }
            }
            return visited;
        }

        public static void Configure(Type type)
        {
            GetSelectorType(type);
        }

        internal static Type GetSelectorType(Type type)
        {
            if (_map.TryGetValue(type, out Type selectorType))
            {
                return selectorType;
            }

            var builders = new Dictionary<Type, DynamicTypeBuilder>();

            var types = FlattTypesMap(type);

            foreach (var t in types)
            {
                if (!_map.ContainsKey(t))
                {
                    builders.Add(t, new DynamicTypeBuilder(t.Name));
                }
            }

            foreach (var builder in builders)
            {
                var properties = GetPropertyInfos(builder.Key);
                foreach (var p in properties)
                {
                    if (p.PropertyType.IsClass && p.PropertyType != typeof(string))
                    {
                        if (_map.TryGetValue(p.PropertyType, out selectorType))
                        {
                            builder.Value.DefineProperty(p.Name, selectorType);
                        }
                        else if (builders.TryGetValue(p.PropertyType, out DynamicTypeBuilder nsbuilder))
                        {
                            builder.Value.DefineProperty(p.Name, nsbuilder);
                        }
                        else
                        {
                            ;
                        }
                         
                    }
                    else
                    {
                        builder.Value.DefineProperty(p);
                    }
                }
            }

            foreach (var builder in builders)
            {
                _map.Add(builder.Key, builder.Value.CreateType());
            }
            return _map[type];
        }

        internal static PropertyInfo[] GetPropertyInfos(Type clrType)
        {
            if (_typesProperties.TryGetValue(clrType, out PropertyInfo[] infos))
            {
                return infos;
            }

            infos = clrType.GetProperties();
            _typesProperties.Add(clrType, infos);
            return infos;
        }
    }
}