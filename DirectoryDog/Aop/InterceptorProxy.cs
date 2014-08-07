using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DirectoryDog.AopInterceptor
{
    public class InterceptorProxy : IInterceptor
    {
        private void PreProceed(IInvocation invocation)
        {

        }
        private void PostProceed(IInvocation invocation)
        {
            var aops = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(AOPAttribute), true);
            AOPAttribute[] attrs = aops as AOPAttribute[];
            foreach (var item in attrs)
            {
                item.Execute(invocation.InvocationTarget);
            }
        }

        public void Intercept(IInvocation invocation)
        {
            this.PreProceed(invocation);
            invocation.Proceed();
            this.PostProceed(invocation);
        }
    }
}
