using System;

using System.ServiceModel;
using MilestoneTG.WcfLatencySimulator.Wcf;

namespace MilestoneTG.WcfLatencySimulator.Library
{
    internal class SimulatorService : ISimulatorService
    {
        public void Report()
        {
            throw new System.ServiceModel.FaultException("Method not implemented.");
        }

        public LatencySettings GetSettings()
        {
            return Simulator.Instance.GetSettings();
        }

    }//end class
}//end namespace
