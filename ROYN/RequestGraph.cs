using System;

namespace ROYN
{
    public sealed class RequestGraph
    {
        //private static List<Property> GetProperties(Type type, string[] propertiesPaths)
        //{
        //    if (type == null)
        //    {
        //        throw new ArgumentNullException(nameof(type));
        //    }

        //    if (propertiesPaths == null)
        //    {
        //        //TODO: return all properties
        //        //return new List<Property>();
        //        throw new NotSupportedException("In Royn include Complex types not supported, you should specify at least one simple property");
        //    }

        //    var NavigationProperties = propertiesPaths.Where(x => x.Contains(".")).GroupBy(x => x.Split('.').FirstOrDefault()).Select(g => new
        //    {
        //        PropertyName = g.Key,
        //        NestedProperties = g.Select(x => x.ReplaceFirst($"{g.Key}.", string.Empty)).ToList()
        //    }).ToList();

        //    var simpleProperties = propertiesPaths.Where(x => !x.Contains(".")).ToList();

        //    var result = new List<Property>();
        //    foreach (var p in NavigationProperties)
        //    {
        //        var pinfo = type.GetProperty(p.PropertyName);
        //        if (p.NestedProperties.Count == 0)
        //        {
        //            if (pinfo.PropertyType.IsClass && pinfo.PropertyType != typeof(string))
        //            {
        //                var complexProperty = new ComplexProperty(pinfo, type, GetProperties(pinfo.PropertyType, null));
        //                result.Add(complexProperty);
        //            }
        //            else
        //            {
        //                result.Add(new PrimitiveProperty(pinfo, type));
        //            }
        //        }
        //        else
        //        {
        //            var complexProperty = new ComplexProperty(pinfo, type, GetProperties(pinfo.PropertyType, p.NestedProperties.ToArray()));
        //            result.Add(complexProperty);
        //        }
        //    }

        //    foreach (var p in simpleProperties)
        //    {
        //        var pinfo = type.GetProperty(p);

        //        if (pinfo.PropertyType.IsClass && pinfo.PropertyType != typeof(string))
        //        {
        //            var complexProperty = new ComplexProperty(pinfo, type, GetProperties(type, null));
        //            result.Add(complexProperty);
        //        }
        //        else
        //        {
        //            result.Add(new PrimitiveProperty(pinfo, type));
        //        }
        //    }

        //    return result;
        //}

        public static RequestGraph BuildGraph(RoynRequest roynRequest, IRoynTypeNameResolver roynTypeNameResolver)
        {
            var clrType = roynTypeNameResolver.Resolve(roynRequest.TypeName);
            var type = Royn.GetSelectorType(clrType);

            return new RequestGraph
            {
                CLRType = clrType,
                SelectType = type,
                Members = roynRequest.Columns.ToArray()
            };
        }

        public Type SelectType = null;

        //public Type BuildType()
        //{
        //    if (SelectType != null)
        //    {
        //        return SelectType;
        //    }

        //    var typeBuilder = new DynamicTypeBuilder(CLRType.Name);
        //    foreach (var p in Properties)
        //    {
        //        if (p is PrimitiveProperty primitive)
        //        {
        //            typeBuilder.DefineProperty(p.Info);
        //        }
        //        else if (p is ComplexProperty complex)
        //        {
        //            typeBuilder.DefineProperty(p.Info.Name, BuildType(complex));
        //        }
        //    }
        //    return SelectType = typeBuilder.CreateType();
        //}

        //private static DynamicTypeBuilder BuildType(PropertyInfo propertyInfo)
        //{
        //    var typeBuilder = new DynamicTypeBuilder($"{propertyInfo.PropertyType.Name}{Guid.NewGuid().ToString("N")}");
        //    foreach (var p in property.Properties)
        //    {
        //        if (p is PrimitiveProperty primitive)
        //        {
        //            typeBuilder.DefineProperty(p.Info);
        //        }
        //        else if (p is ComplexProperty complex)
        //        {
        //            typeBuilder.DefineProperty(p.Info.Name, BuildType(complex));
        //        }
        //    }
        //    return typeBuilder;
        //}

        public static RequestGraph BuildGraph<T>(RoynRequest<T> roynRequest)
        {
            //TODO use DI to get RoynTypeNameResolver from the configuration
            var graph = new RequestGraph
            {
                CLRType = roynRequest.CLRType,
                SelectType = Royn.GetSelectorType(roynRequest.CLRType),
                Members = roynRequest.Columns.ToArray()
            };

            return graph;
        }

        public TypeName TypeName { get; private set; }
        public Type CLRType { get; private set; }
        public string[] Members { get; private set; }
    }

    //public abstract class Property
    //{
    //    public PropertyInfo Info { get; private set; }
    //    public readonly Type OwnerType;

    //    public Property(PropertyInfo propertyinfo, Type ownerType)
    //    {
    //        this.Info = propertyinfo;
    //        OwnerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));
    //    }
    //}

    //public class PrimitiveProperty : Property
    //{
    //    public PrimitiveProperty(PropertyInfo propertyinfo, Type type) : base(propertyinfo, type)
    //    {
    //    }
    //}

    //public class ComplexProperty : Property
    //{
    //    public ComplexProperty(PropertyInfo propertyinfo, Type type) : base(propertyinfo, type)
    //    {
    //        Properties = new List<Property>();
    //    }

    //    public ComplexProperty(PropertyInfo propertyinfo, Type type, List<Property> properties) : this(propertyinfo, type, properties?.ToArray())
    //    {
    //    }

    //    public ComplexProperty(PropertyInfo propertyinfo, Type type, Property[] properties) : this(propertyinfo, type)
    //    {
    //        if (properties == null)
    //        {
    //            throw new ArgumentNullException(nameof(properties));
    //        }

    //        Properties = new List<Property>(properties);
    //    }

    //    public List<Property> Properties { get; protected set; }
    //}
}