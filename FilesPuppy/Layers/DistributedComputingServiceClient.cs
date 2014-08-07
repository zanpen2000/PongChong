using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Runtime.Serialization;

namespace FilesPuppy.Layers
{
    public class DistributedComputingServiceClient
    {

        public static ChannelFactory<ISvc> CreateWebChannelFactory<ISvc>(string RemoteAddress, string ServiceName)
        {
            BasicHttpBinding theBinding = new BasicHttpBinding();
            theBinding.MaxReceivedMessageSize = int.MaxValue;
            theBinding.MaxBufferSize = int.MaxValue;
            theBinding.MaxBufferPoolSize = int.MaxValue;
            theBinding.ReaderQuotas.MaxDepth = 32;
            theBinding.ReaderQuotas.MaxStringContentLength = 2147483647;
            theBinding.ReaderQuotas.MaxArrayLength = 2147483647;
            theBinding.ReaderQuotas.MaxBytesPerRead = 2147483647;
            theBinding.ReaderQuotas.MaxNameTableCharCount = 2147483647;
            theBinding.Security.Mode = BasicHttpSecurityMode.None;
            theBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            
            return new ChannelFactory<ISvc>(theBinding, RemoteAddress);

        }

    }
}
