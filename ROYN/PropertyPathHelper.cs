using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ROYN
{
    public static class PropertyPathHelper
    {
        internal static Dictionary<Type, List<PropertyInfo>> ResolveIncludeProperties(this Type type, string[] includedPaths = null)
        {
            var includedProperties = new Dictionary<Type, List<PropertyInfo>>();
            if (includedPaths != null)
            {
                foreach (var path in includedPaths)
                {
                    var propName = path;
                    if (path.Contains("."))
                    {
                        var paths = path.Split('.');
                        var _type = type.GetProperty(paths[0]).PropertyType;
                        if (includedProperties.ContainsKey(type))
                        {
                            if (!includedProperties[type].Any(x => x.Name == paths[0]))
                            {
                                includedProperties[type].Add(type.GetProperty(paths[0]));
                            }
                        }
                        else
                        {
                            includedProperties.Add(type, new List<PropertyInfo> { type.GetProperty(paths[0]) });
                        }

                        for (int i = 1; i < paths.Length; i++)
                        {
                            var segment = paths[i];
                            if (includedProperties.ContainsKey(_type))
                            {
                                if (!includedProperties[_type].Any(x => x.Name == segment))
                                {
                                    includedProperties[_type].Add(_type.GetProperty(segment));
                                }
                            }
                            else
                            {
                                includedProperties.Add(_type, new List<PropertyInfo> { _type.GetProperty(segment) });
                            }
                            _type = _type.GetProperty(paths[i]).PropertyType;
                        }
                    }
                    else
                    {
                        if (includedProperties.ContainsKey(type))
                        {
                            if (!includedProperties[type].Any(x => x.Name == propName))
                            {
                                includedProperties[type].Add(type.GetProperty(propName));
                            }
                        }
                        else
                        {
                            includedProperties.Add(type, new List<PropertyInfo> { type.GetProperty(propName) });
                        }
                    }
                }
            }

            return includedProperties;
        }
    }
}