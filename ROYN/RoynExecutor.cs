using System;
using System.Data.Entity;

namespace ROYN
{
    public class RoynExecutor : IDisposable
    {
        public IRoynTypeNameResolver TypeNameResolver { get; set; } = new TypeResolver();

        //public RoynResult Execute<T>(IQueryable<T> source, RoynRequest<T> roynRequest) where T : class
        //{
        //    return RoynHelper.RoynSelect(source, roynRequest);
        //}

        public RoynResult Execute<T>(DbSet<T> source, RoynRequest roynRequest) where T : class
        {
            return RoynHelper.RoynSelect(source, roynRequest.AsGeneric<T>());
        }

        public RoynResult Execute<T, TResult>(DbSet<T> source, RoynRequest roynRequest)
            where T : class
            where TResult : class
        {
            return RoynHelper.RoynSelect<T, TResult>(source, roynRequest.AsGeneric<T>());
        }

        public RoynResult Execute<T, TResult>(DbSet<T> source, RoynRequest<T> roynRequest, RequestGraph graph)
           where T : class
           where TResult : class
        {
            return RoynHelper.RoynSelect<T, TResult>(source, roynRequest, graph);
        }

        public RoynResult Execute<T, TResult>(DbSet<T> source, RoynRequest roynRequest, RequestGraph graph)
        where T : class
        where TResult : class
        {
            return RoynHelper.RoynSelect<T, TResult>(source, roynRequest.AsGeneric<T>(), graph);
        }

        public RoynResult Execute<T>(DbSet<T> source, RoynRequest roynRequest, RequestGraph graph)
       where T : class
        {
            return RoynHelper.RoynSelect<T>(source, roynRequest.AsGeneric<T>(), graph);
        }

        public void Dispose()
        {
        }
    }
}