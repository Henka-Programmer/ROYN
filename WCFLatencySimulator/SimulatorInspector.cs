using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Xml;

namespace MilestoneTG.WcfLatencySimulator.Wcf
{
    /// <summary>
    /// A System.ServiceModel.Dispatcher.IClientMessageInspector that applies latency and bandwidth restrictions specified by the Simulator
    /// to the send/receive of the message to/from the server.
    /// </summary>
    public class SimulatorInspector : System.ServiceModel.Dispatcher.IClientMessageInspector
    {
        private ISimulatorService _simulatorService;

        public SimulatorInspector()
        {
            NetNamedPipeBinding binding = new NetNamedPipeBinding();

            EndpointAddress endpointAddress = new EndpointAddress("net.pipe://localhost/MilestoneTG/WcfLatencySimulator");

            ChannelFactory<ISimulatorService> channelFactory = new ChannelFactory<ISimulatorService>(binding, endpointAddress);

            // Create a channel.
            _simulatorService = channelFactory.CreateChannel();
        }

        private System.ServiceModel.Channels.Message ApplySettings(System.ServiceModel.Channels.Message pMessage)
        {
            int latency = 0;
            long bandwidth = int.MaxValue;

            try
            {
                //Get the current settings from the tool
                LatencySettings settings = _simulatorService.GetSettings();

                latency = settings.Latency;
                bandwidth = settings.Bandwidth;
            }
            catch
            {
                //use defaults

                //TODO:  Implement logging
            }

            //Calculations...

            MessageBuffer copy = pMessage.CreateBufferedCopy(int.MaxValue);

            Message msg = copy.CreateMessage();

            Message newMessage = copy.CreateMessage();

            MemoryStream stream = new MemoryStream();

            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(stream);

            msg.WriteMessage(writer);

            writer.Flush();

            long soapMessageSize = stream.Length;

            writer.Close();

            long bandwidthRate = (soapMessageSize * 8 / (bandwidth == 0 ? 1 : bandwidth)) * 1000; //times byte rate time 1000 milliseconds in one second times 8 bits in 1 byte

            TimeSpan wait = TimeSpan.FromMilliseconds(bandwidthRate + latency);
            //Apply the bandwidth
            Thread.Sleep(wait);

            return newMessage;
        }

        #region IClientMessageInspector Members

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            reply = ApplySettings(reply);
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            request = ApplySettings(request);
            return null;
        }

        #endregion IClientMessageInspector Members
    }//end class
}//end namespace