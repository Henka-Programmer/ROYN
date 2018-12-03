using System;
using System.Runtime.Serialization;

namespace MilestoneTG.WcfLatencySimulator.Wcf
{
    [DataContract]
    public class LatencySettings
    {
        [DataMember]
        public int Latency { get; set; }

        [DataMember]
        public long Bandwidth { get; set; }
    }
}
