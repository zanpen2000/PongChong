using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;


namespace AppLayer.Layers
{
    public class ServiceCaller
    {



        public static void ServiceExecute<ISvc>(Action<ISvc> ac)
        {
            
            string addr = AppSettings.Get("ServiceAddress");
            if (string.IsNullOrEmpty(addr))
            {
                throw new ArgumentNullException("未能获取Web Service地址");
            }
            ServiceExecute<ISvc>(addr, ac);

        }

        public static void ServiceExecute<ISvc>(string addr, Action<ISvc> ac)
        {
            try
            {
                using (var factory = DistributedComputingServiceClient.CreateWebChannelFactory<ISvc>(addr, null))
                {
                    
                    ISvc proxy = factory.CreateChannel();
                    ac(proxy);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
