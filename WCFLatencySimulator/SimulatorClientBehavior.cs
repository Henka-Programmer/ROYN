using System;

namespace MilestoneTG.WcfLatencySimulator.Wcf
{
    /// <summary>
    /// An IEndpointBehavior that adds the SimulatorInspector to the ClientRuntime's MessageInspector collection.
    /// </summary>
    public class SimulatorClientBehavior : System.ServiceModel.Description.IEndpointBehavior 
    {
        #region IEndpointBehavior Members

        public void AddBindingParameters(System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
           
        }

        public void ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            //add the Inspector to the client
            clientRuntime.MessageInspectors.Add(new SimulatorInspector());
        }

        public void ApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            
        }

        public void Validate(System.ServiceModel.Description.ServiceEndpoint endpoint)
        {
            
        }

        #endregion
    }
}
