using System;
using System.Data.Entity;
using System.Linq;

namespace ROYN
{
    public class RoynExecutor : IDisposable
    {
        public RoynResult Execute<T>(IQueryable<T> source, RoynRequest<T> roynRequest) where T : class
        {
            return RoynHelper.RoynSelect<T>(source, roynRequest);
        }

        public RoynResult Execute<T>(DbSet<T> source, RoynRequest<T> roynRequest) where T : class
        {
            return RoynHelper.RoynSelect<T>(source, roynRequest);
        }

        public void Dispose()
        {
        }
    }
}