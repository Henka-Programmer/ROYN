using System;
using System.Collections.Generic;
using System.Reflection;

namespace ROYN
{
    public sealed class RequestGraph
    {
        public static RequestGraph BuildGraph(RoynRequest roynRequest, IRoynTypeNameResolver roynTypeNameResolver)
        {
            //TODO use DI to get RoynTypeNameResolver from the configuration
            var graph = new RequestGraph
            {
                CLRType = roynTypeNameResolver.Resolve(roynRequest.TypeName)
            };

            //foreach (var keyValuePair in selectFields)
            //{
            //    var propertyType = propertyInfos
            //        .FirstOrDefault(p => p.Name.ToLowerInvariant().Equals(keyValuePair.Key.ToLowerInvariant()))
            //        .PropertyType;

            //    if (propertyType.IsClass && propertyType != typeof(string))
            //    {
            //    }
            //}

            return null;
        }

        public TypeName TypeName { get; private set; }
        public Type CLRType { get; private set; }
        public List<Property> Properties { get; private set; }
    }

    public abstract class Property
    {
        private readonly PropertyInfo propertyinfo;
        private readonly Type type;

        public Property(PropertyInfo propertyinfo, Type type)
        {
            this.propertyinfo = propertyinfo;
            this.type = type;
        }

        protected Property()
        {
        }
    }

    public class PrimitiveProperty : Property
    {
        public PrimitiveProperty(PropertyInfo propertyinfo, Type type) : base(propertyinfo, type)
        {
        }
    }

    public class ComplexProperty : Property
    {
        public ComplexProperty(PropertyInfo propertyinfo, Type type) : base(propertyinfo, type)
        {
        }

        public List<Property> Properties { get; protected set; }
    }
}