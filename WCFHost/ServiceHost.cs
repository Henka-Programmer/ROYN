using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace WCFClient
{
    public class ServiceHost<T, IT> : ServiceHost, IDisposable
   where T : class
    {
        /// <summary>
        /// return with the Singleton HostedService Instance.
        /// </summary>
        public T ServiceInstance
        {
            get
            {
                try
                {
                    return (T)SingletonInstance;
                }
                catch (Exception)
                {
                    return default(T);
                }
            }
        }

        public ServiceHost(Binding binding, Uri uri) : base(typeof(T), uri)
        {
            _InitializeEndPoints(binding);
        }

        private void _InitializeEndPoints(Binding binding) => InitializeEndPoints(binding);

        protected virtual void InitializeEndPoints(Binding binding)
        {
            var ep = AddServiceEndpoint(typeof(IT), binding, "");

            var stp = Description.Behaviors.Find<ServiceDebugBehavior>();
            stp.HttpHelpPageEnabled = false;
        }

        public void Dispose()
        {
            Close();
            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}