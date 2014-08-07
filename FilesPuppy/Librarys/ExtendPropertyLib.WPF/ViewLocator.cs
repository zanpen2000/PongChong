using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtendPropertyLib.WPF;
using System.Windows;
using System.Reflection;

namespace ExtendPropertyLib.WPF
{
    public class ViewLocator
    {
        public FrameworkElement FindView(object viewModel)
        {
            string viewModelName = viewModel.GetType().FullName;
            Assembly assembly = null;
           
            //动态代理
            if (viewModelName.StartsWith("TEMP"))
            {
                assembly = viewModel.GetType().BaseType.Assembly;
                viewModelName = viewModel.GetType().BaseType.FullName;
            }
            else
            {
                assembly = viewModel.GetType().Assembly;
            }

            string viewName = viewModelName.Replace("Model", "");
            viewName = viewName.Replace("TEMP_DYNAMIC_ASSEMBLY__Proxy", "");
            Type viewType = assembly.GetType(viewName);

           
            FrameworkElement view = (FrameworkElement)Activator.CreateInstance(viewType);
            
           
            return view;
        }
    }
}
