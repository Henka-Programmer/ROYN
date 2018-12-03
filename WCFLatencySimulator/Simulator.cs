using System;
using MilestoneTG.WcfLatencySimulator.Wcf;

namespace MilestoneTG.WcfLatencySimulator.Library
{
    /// <summary>
    /// The core engine of the simulator implemented as a lazy singleton.  This class hosts a simple WCF service the SimulatorInspector
    /// uses to retreive the LatencySettings to apply, and to report metrics back to the simulator.
    /// </summary>
    public class Simulator
    {
        private ISettingsProvider _settingsView = null;

        private SimulatorServiceHost _host = null;

        class LazySingleton
        {
            static LazySingleton() { }
            internal static Simulator instance = new Simulator();
        }

        private Simulator() 
        {
            _host = new SimulatorServiceHost();
        }

        /// <summary>
        /// Returns a lazy singleton instance.
        /// </summary>
        public static Simulator Instance {get { return LazySingleton.instance; } }

        /// <summary>
        /// Sets the ISettingsProvider the simulator should use to retrieve the current LatencySettings.
        /// </summary>
        /// <param name="pView"></param>
        public void SetProvider(ISettingsProvider pView)
        {
            _settingsView = pView;
        }

        /// <summary>
        /// Gets the current settings from the ISettingsProvider specified by the SetProvider() method,
        /// </summary>
        /// <returns></returns>
        public LatencySettings GetSettings()
        {
            return _settingsView.GetSettings();
        }

        /// <summary>
        /// Starts the simulator and host service.
        /// </summary>
        public void Start()
        {
            _host.Start();
        }

        /// <summary>
        /// Stops the simulator and host service.
        /// </summary>
        public void Stop()
        {
            _host.Stop();
        }

    }//end class
}//end namesoace
