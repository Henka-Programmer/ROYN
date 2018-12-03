using System;

using MilestoneTG.WcfLatencySimulator.Wcf;

namespace MilestoneTG.WcfLatencySimulator.Library
{
    /// <summary>
    /// This interface must be implemented by all settings providers.  The simulator uses the this interface to retreive the
    /// settings from the provider.
    /// </summary>
    public interface ISettingsProvider
    {
        /// <summary>
        /// When implemented, returns the LegacySettings supplied by the implementing class.
        /// </summary>
        /// <returns></returns>
        LatencySettings GetSettings();
    }
}
