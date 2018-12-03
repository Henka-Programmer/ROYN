using System;
using System.ServiceModel;

namespace MilestoneTG.WcfLatencySimulator.Wcf
{
    [ServiceContract]
    public interface ISimulatorService
    {
        [OperationContract]
        void Report();

        [OperationContract]
        LatencySettings GetSettings();
    }
}
