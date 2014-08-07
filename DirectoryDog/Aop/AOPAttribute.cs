using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectoryDog.AopInterceptor
{
    public abstract class AOPAttribute : Attribute
    {

        public virtual void Execute(params object[] objects) { return; }

    }
}
