using System;
using System.ServiceModel;

namespace WCFCommon
{
    public class DefaultBasicHttpBinding : BasicHttpBinding
    {
        public DefaultBasicHttpBinding()
            : base(BasicHttpSecurityMode.None)
        {
            MaxBufferPoolSize = 1073741824;
            MaxBufferSize = 1073741824;
            MaxReceivedMessageSize = 1073741824;
            TransferMode = TransferMode.Streamed;
            ReaderQuotas.MaxArrayLength = 1073741824;
            ReaderQuotas.MaxBytesPerRead = 1073741824;
            ReaderQuotas.MaxStringContentLength = 1073741824;
            UseDefaultWebProxy = false;
            ReceiveTimeout = TimeSpan.FromSeconds(20);
            SendTimeout = TimeSpan.FromSeconds(20);
        }
    }
}