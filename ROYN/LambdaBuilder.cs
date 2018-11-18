using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ROYN
{
    public class LambdaBuilder
    {
        //private Dictionary<string, List<string>> GetFieldMapping(string[] fields)
        //{
        //    var selectedFieldsMap = new Dictionary<string, List<string>>();

        //    foreach (var s in fields)
        //    {
        //        var nestedFields = s.Split('.').Select(f => f.Trim()).ToArray();
        //        var nestedValue = nestedFields.Length > 1 ? nestedFields[1] : null;

        //        if (selectedFieldsMap.Keys.Any(key => key == nestedFields[0]))
        //        {
        //            selectedFieldsMap[nestedFields[0]].Add(nestedValue);
        //        }
        //        else
        //        {
        //            selectedFieldsMap.Add(nestedFields[0], new List<string> { nestedValue });
        //        }
        //    }

        //    return selectedFieldsMap;
        //}

        //private static ConditionalExpression NullPropagate(Expression baseExpr, Expression returnExpr)
        //{
        //    var equals = Expression.Equal(baseExpr, Expression.Constant(null));
        //    return Expression.Condition(equals, Expression.Constant(null, returnExpr.Type), returnExpr);
        //}

        //private void HandleComplexProperty(Property np, ComplexProperty property, ref List<MemberAssignment> assignments, ParameterExpression parameter)
        //{
        //    if (property is ComplexProperty complex)
        //    {
        //        ParameterExpression xParameter = Expression.Parameter(complex.OwnerType, $"{complex.Info.PropertyType.Name}{complex.Info.Name}");

        //        var objNestedMemberExpression = Expression.Property(parameter, complex.Info);

        //        NewExpression innerObjNew = Expression.New(complex.Info.PropertyType);
        //        var nestedBindings = new List<MemberAssignment>();

        //        foreach (var np in complex.Properties)
        //        {
        //            HandleComplexProperty(np, ref nestedBindings, xParameter, objNestedMemberExpression);
        //        }

        //        var nestedInit = Expression.MemberInit(innerObjNew, nestedBindings);
        //        assignments.Add(Expression.Bind(complex.Info, NullPropagate(objNestedMemberExpression, nestedInit)));
        //    }
        //}

        public static Expression<Func<TSource, TTarget>> BuildSelector<TSource, TTarget>(string members)
        {
            return BuildSelector<TSource, TTarget>(members.Split(',').Select(m => m.Trim()));
        }

        public static Expression<Func<TSource, TTarget>> BuildSelector<TSource, TTarget>(IEnumerable<string> members)
        {
            var parameter = Expression.Parameter(typeof(TSource), "e");
            var body = NewObject(typeof(TTarget), parameter, members.Select(m => m.Split('.')));
            return Expression.Lambda<Func<TSource, TTarget>>(body, parameter);
        }

        public static Expression<Func<TSource, TTarget>> BuildSelector<TSource, TTarget>(RequestGraph graph)
        {
            var parameter = Expression.Parameter(typeof(TSource), "e");
            var body = NewObject(typeof(TTarget), parameter, graph.Properties); 
            return Expression.Lambda<Func<TSource, TTarget>>(body, parameter);
        }

        private static ConditionalExpression NullPropagate(Expression baseExpr, Expression returnExpr)
        {
            var equals = Expression.Equal(baseExpr, Expression.Constant(null));
            return Expression.Condition(equals, Expression.Constant(null, returnExpr.Type), returnExpr);
        }
        private static Expression NewObject(Type targetType, Expression source, List<Property> properties)
        {
            var bindings = new List<MemberBinding>();
            var target = Expression.Constant(null, targetType);

            foreach (var p in properties)
            {
                var memberName = p.Info.Name;
                var targetMember = Expression.PropertyOrField(target, memberName);
                var sourceMember = Expression.PropertyOrField(source, memberName);

                if (p is PrimitiveProperty primitive)
                {
                    bindings.Add(Expression.Bind(targetMember.Member, sourceMember));
                }
                else if (p is ComplexProperty complex)
                {
                    var childMembers = complex.Properties;
                    var targetValue = !childMembers.Any() ? sourceMember :
                        NewObject(targetMember.Type, sourceMember, childMembers);
                    bindings.Add(Expression.Bind(targetMember.Member, NullPropagate(sourceMember, targetValue)));
                }
            }

            return Expression.MemberInit(Expression.New(targetType), bindings);
        }

        private static Expression NewObject(Type targetType, Expression source, IEnumerable<string[]> memberPaths, int depth = 0)
        {
            var bindings = new List<MemberBinding>();
            var target = Expression.Constant(null, targetType);
            foreach (var memberGroup in memberPaths.GroupBy(path => path[depth]))
            {
                var memberName = memberGroup.Key;
                var targetMember = Expression.PropertyOrField(target, memberName);
                var sourceMember = Expression.PropertyOrField(source, memberName);
                var childMembers = memberGroup.Where(path => depth + 1 < path.Length);
                var targetValue = !childMembers.Any() ? sourceMember :
                    NewObject(targetMember.Type, sourceMember, childMembers, depth + 1);
                bindings.Add(Expression.Bind(targetMember.Member, targetValue));
            }
            return Expression.MemberInit(Expression.New(targetType), bindings);
        }
    }
}