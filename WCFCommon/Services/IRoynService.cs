using ROYN;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace WCFCommon.Services
{
    [ServiceContract]
    [ServiceKnownType(typeof(RoynRequest))]
    [ServiceKnownType(typeof(RoynResult))]
    [ServiceKnownType(typeof(TypeName))]
    [ServiceKnownType(typeof(SortDirection))]
    [ServiceKnownType(typeof(ResolveOptions))]
    public interface IRoynService
    {
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        RoynResult Execute(RoynRequest request);
    }
}