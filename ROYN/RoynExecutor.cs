using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

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

        //public RoynResult Execute<T>(DbSet source, RoynRequest roynRequest) where T : class
        //{
        //    return RoynHelper.RoynSelect(source as DbSet<T>, roynRequest.AsGeneric<T>());
        //}

        public void Dispose()
        {
        }
    }
}