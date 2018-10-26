using System.Data.Entity;

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
    }
}