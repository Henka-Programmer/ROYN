using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using MilestoneTG.WcfLatencySimulator.Wcf;

namespace MilestoneTG.WcfLatencySimulator.Library
{
    internal class SimulatorServiceHost
    {
        private ServiceHost _host = null;

        internal void Start()
        {
            if (_host == null)
            {
                Initialize();
            }
        }

        internal void Stop()
        {
            if (_host != null)
            {
                _host.Close();
            }
        }

        private void Initialize()
        {
            _host = new ServiceHost(typeof(SimulatorService), new Uri("net.pipe://localhost/MilestoneTG/WcfLatencySimulator"));

            _host.Faulted += new EventHandler(OnHostFaulted);

            ServiceEndpoint endPoint = _host.AddServiceEndpoint(typeof(ISimulatorService), new NetNamedPipeBinding(), "");

            _host.Open();
        }

        private void OnHostFaulted(object sender, EventArgs e)
        {
            try
            {
                _host.Close();
            }
            catch { }

            _host = null;

            Initialize();
        }
    }
}
