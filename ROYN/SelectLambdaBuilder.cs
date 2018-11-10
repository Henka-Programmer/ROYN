using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ROYN
{
    public class SelectLambdaBuilder
    {
        // as a performence consideration I cached already computed type-properties
        private static Dictionary<Type, PropertyInfo[]> _typePropertyInfoMappings = new Dictionary<Type, PropertyInfo[]>();

        private readonly Type _typeOfBaseClass;

        public SelectLambdaBuilder(Type type)
        {
            _typeOfBaseClass = type;
            if (!_typePropertyInfoMappings.Keys.Contains(_typeOfBaseClass))
            {
                var properties = _typeOfBaseClass.GetProperties();
                _typePropertyInfoMappings.Add(_typeOfBaseClass, properties);
            }
        }

        private Dictionary<string, List<string>> GetFieldMapping(string[] fields)
        {
            var selectedFieldsMap = new Dictionary<string, List<string>>();

            foreach (var s in fields)
            {
                var nestedFields = s.Split('.').Select(f => f.Trim()).ToArray();
                var nestedValue = nestedFields.Length > 1 ? nestedFields[1] : null;

                if (selectedFieldsMap.Keys.Any(key => key == nestedFields[0]))
                {
                    selectedFieldsMap[nestedFields[0]].Add(nestedValue);
                }
                else
                {
                    selectedFieldsMap.Add(nestedFields[0], new List<string> { nestedValue });
                }
            }

            return selectedFieldsMap;
        }

        private static ConditionalExpression NullPropagate(Expression baseExpr, Expression returnExpr)
        {
            var equals = Expression.Equal(baseExpr, Expression.Constant(null));
            return Expression.Condition(equals, Expression.Constant(null, returnExpr.Type), returnExpr);
        }

        public Expression<Func<T, TResult>> CreateNewStatement<T, TResult>(string[] columns)
        {
            ParameterExpression xParameter = Expression.Parameter(_typeOfBaseClass, "s");
            NewExpression xNew = Expression.New(_typeOfBaseClass);

            var selectFields = GetFieldMapping(columns);

            var shpNestedPropertyBindings = new List<MemberAssignment>();
            if (!_typePropertyInfoMappings.TryGetValue(_typeOfBaseClass, out PropertyInfo[] propertyInfos))
            {
                var properties = _typeOfBaseClass.GetProperties();
                propertyInfos = properties;
                _typePropertyInfoMappings.Add(_typeOfBaseClass, properties);
            }

            foreach (var keyValuePair in selectFields)
            {
                var propertyType = propertyInfos
                    .FirstOrDefault(p => p.Name.ToLowerInvariant().Equals(keyValuePair.Key.ToLowerInvariant()))
                    .PropertyType;

                if (propertyType.IsClass && propertyType != typeof(string))
                {
                    PropertyInfo objClassPropInfo = _typeOfBaseClass.GetProperty(keyValuePair.Key);
                    MemberExpression objNestedMemberExpression = Expression.Property(xParameter, objClassPropInfo);

                    NewExpression innerObjNew = Expression.New(propertyType);

                    var nestedBindings = keyValuePair.Value.Select(v =>
                    {
                        PropertyInfo nestedObjPropInfo = propertyType.GetProperty(v);

                        MemberExpression nestedOrigin2 = Expression.Property(objNestedMemberExpression, nestedObjPropInfo);
                        var binding2 = Expression.Bind(nestedObjPropInfo, nestedOrigin2);

                        return binding2;
                    });

                    var nestedInit = Expression.MemberInit(innerObjNew, nestedBindings);
                    shpNestedPropertyBindings.Add(Expression.Bind(objClassPropInfo, NullPropagate(objNestedMemberExpression, nestedInit)));
                }
                else
                {
                    Expression mbr = xParameter;
                    mbr = Expression.PropertyOrField(mbr, keyValuePair.Key);

                    PropertyInfo mi = _typeOfBaseClass.GetProperty(((MemberExpression)mbr).Member.Name);

                    var xOriginal = Expression.Property(xParameter, mi);

                    shpNestedPropertyBindings.Add(Expression.Bind(mi, xOriginal));
                }
            }

            var xInit = Expression.MemberInit(xNew, shpNestedPropertyBindings);
            var lambda = Expression.Lambda<Func<T, TResult>>(xInit, xParameter);

            return lambda;
        }
    }
}