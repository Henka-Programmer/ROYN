using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ROYN
{
    public class PropertyIncludeSerializerContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, HashSet<string>> _includes;
        private readonly Dictionary<Type, Dictionary<string, string>> _renames;

        public PropertyIncludeSerializerContractResolver()
        {
            _includes = new Dictionary<Type, HashSet<string>>();
            _renames = new Dictionary<Type, Dictionary<string, string>>();
        }

        public void IncludeProperty(Type type, PropertyInfo[] propertiesInfos)
        {
            if (!_includes.ContainsKey(type))
                _includes[type] = new HashSet<string>();

            foreach (var prop in propertiesInfos)
            {
                //if (!_includes.ContainsKey(prop.DeclaringType))
                //    _includes[prop.DeclaringType] = new HashSet<string>();
                _includes[type].Add(prop.Name);
            }
        }

        public void RenameProperty(Type type, string propertyName, string newJsonPropertyName)
        {
            if (!_renames.ContainsKey(type))
                _renames[type] = new Dictionary<string, string>();

            _renames[type][propertyName] = newJsonPropertyName;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            foreach (var property in properties)
            {
                if (IsIgnored(type, property.PropertyName))
                {
                    property.ShouldDeserialize = i => false;
                    property.Ignored = true;
                }
            }
            return properties;
        }

        private bool IsIgnored(Type type, string jsonPropertyName)
        {
            if (!_includes.ContainsKey(type))
                return true;

            return !_includes[type].Contains(jsonPropertyName);
        }

        private bool IsRenamed(Type type, string jsonPropertyName, out string newJsonPropertyName)
        {
            Dictionary<string, string> renames;

            if (!_renames.TryGetValue(type, out renames) || !renames.TryGetValue(jsonPropertyName, out newJsonPropertyName))
            {
                newJsonPropertyName = null;
                return false;
            }

            return true;
        }
    }
}