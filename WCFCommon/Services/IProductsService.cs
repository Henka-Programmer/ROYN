using ROYN;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace WCFCommon.Services
{
    [ServiceContract]
    [ServiceKnownType(typeof(Product))]
    [ServiceKnownType(typeof(ModelBase))]
    [ServiceKnownType(typeof(ProductCategory))]
    [ServiceKnownType(typeof(ProductType))]
    [ServiceKnownType(typeof(MeasureUnit))]
    [ServiceKnownType(typeof(MeasureUnitCategory))]
    [ServiceKnownType(typeof(RoynRequest))]
    [ServiceKnownType(typeof(RoynRequest<Product>))]
    [ServiceKnownType(typeof(RoynResult))]
    [ServiceKnownType(typeof(TypeName))]
    public interface IProductsService : IRoynService
    {
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        List<Product> GetProducts(int size);
    }
}