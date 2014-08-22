using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace WpfWcfServer
{
    public class MyServiceHost : ServiceHost
    {
        private Type type;

        public MyServiceHost(Type type):base(type)
        {
            // TODO: Complete member initialization
            this.type = type;
        }
        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();


        }
    }
}
