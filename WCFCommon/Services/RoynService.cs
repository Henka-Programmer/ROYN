using ROYN;

namespace WCFCommon.Services
{
    public abstract class RoynService : IRoynService
    {
        public abstract RoynResult Execute(RoynRequest request);
    }
}