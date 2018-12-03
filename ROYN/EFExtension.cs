using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace ROYN
{
    public static class EFExtension
    {
        public static RoynResult Execute<T>(this DbContext context, RoynRequest<T> roynRequest) where T : class
        {
            using (var excutor = new RoynExecutor())
            {
                roynRequest.CLRType = typeof(T);
                return excutor.Execute(context.Set<T>(), roynRequest);
            }
        }

        public static RoynResult Execute<T>(this DbContext context, RoynRequest roynRequest) where T : class
        {
            using (var excutor = new RoynExecutor())
            {
                return excutor.Execute(context.Set<T>(), roynRequest.AsGeneric<T>());
            }
        }

        public static MethodInfo GetGenericMethod(this Type t, string name, Type[] genericArgTypes, Type[] argTypes, Type returnType)
        {
            MethodInfo foo1 = (from m in t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                               where m.Name == name
                               && m.GetGenericArguments().Length == genericArgTypes.Length
                               && m.GetParameters().Select(pi => pi.ParameterType.IsGenericType ? pi.ParameterType.GetGenericTypeDefinition() : pi.ParameterType).SequenceEqual(argTypes) &&
                               (returnType == null || (m.ReturnType.IsGenericType ? m.ReturnType.GetGenericTypeDefinition() : m.ReturnType) == returnType)
                               select m).FirstOrDefault();

            if (foo1 != null)
            {
                return foo1.MakeGenericMethod(genericArgTypes);
            }
            return null;
        }

        public static RoynResult Execute(this DbContext context, RoynRequest roynRequest)
        {
            using (var excutor = new RoynExecutor())
            {
                var graph = RequestGraph.BuildGraph(roynRequest, excutor.TypeNameResolver);
                MethodInfo genericExecuteMethod = typeof(RoynExecutor).GetGenericMethod("Execute", new Type[] { graph.CLRType, graph.SelectType }, new Type[] { typeof(DbSet<>), typeof(RoynRequest), typeof(RequestGraph) }, typeof(RoynResult));   //executeMethod.MakeGenericMethod(type);

                MethodInfo genericDbSetMethod = typeof(DbContext).GetGenericMethod("Set", new Type[] { graph.CLRType }, new Type[] { }, typeof(DbSet<>));

                return (RoynResult)genericExecuteMethod.Invoke(excutor, new object[] { genericDbSetMethod.Invoke(context, null), roynRequest, graph });
            }
        }
    }
}