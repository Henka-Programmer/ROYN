using System;
using System.Data.Entity;
using System.Reflection;

namespace ROYN
{
    public static class EFExtension
    {
        public static RoynResult Execute<T>(this DbContext context, RoynRequest<T> roynRequest) where T : class
        {
            using (var excutor = new RoynExecutor())
            {
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

        public static RoynResult Execute(this DbContext context, RoynRequest roynRequest)
        {
            using (var excutor = new RoynExecutor())
            {
                var type = excutor.TypeNameResolver.Resolve(roynRequest.TypeName);
                MethodInfo executeMethod = typeof(RoynExecutor).GetMethod("Execute", new Type[] { typeof(DbSet<>).MakeGenericType(type), typeof(RoynRequest) });
                MethodInfo genericExecuteMethod = executeMethod.MakeGenericMethod(type);

                //MethodInfo dbSetMethod = typeof(DbContext).GetMethod("Set", new Type[] { typeof(DbSet<>).MakeGenericType(type), typeof(RoynRequest) });
                //MethodInfo genericExecuteMethod = executeMethod.MakeGenericMethod(type);

                //return (RoynResult)genericExecuteMethod.Invoke(excutor, new object[] { source, roynRequest });
                return excutor.Execute(type, context.Set(type), roynRequest);
            }
        }
    }
}