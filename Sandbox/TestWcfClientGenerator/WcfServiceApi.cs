using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace TestWcfClientGenerator
{
    // Create custom part before generation
    public partial class WcfServiceApi
    {
        private readonly IAPIConfig _config;
        private readonly EndpointAddress _endPoint;
        private readonly BasicHttpBinding _binding;

        public WcfServiceApi(IAPIConfig config)
        {
            _config = config;
            _binding = CreateBasicHttpBinding();
            _endPoint = new EndpointAddress(_config.WcfServiceAddress);
        }

        #region Private methods

        private BasicHttpBinding CreateBasicHttpBinding()
        {
            var binding = new BasicHttpBinding
            {
                Name = "basicHttpBinding",
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647
            };

            var timeout = new TimeSpan(0, 10, 0);
            binding.SendTimeout = timeout;
            binding.OpenTimeout = timeout;
            binding.ReceiveTimeout = timeout;
            if (_config.WcfServiceAddress.StartsWith("https://"))
            {
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
            }
            return binding;
        }

        private void AddClientInformationHeader()
        {
            var requestMessage = new HttpRequestMessageProperty();
            requestMessage.Headers["TestHeader"] = "Test";
            OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
        }

        #endregion
    }
}