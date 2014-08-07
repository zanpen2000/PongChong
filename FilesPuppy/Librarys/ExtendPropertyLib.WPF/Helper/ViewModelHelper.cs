using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtendPropertyLib.WPF;
using System.Windows;

namespace ExtendPropertyLib.WPF.Helper
{
    public static  class ViewModelHelper
    {
        public static void TryOK(this ViewModelBase viewModel)
        {
           var elem = viewModel.View as FrameworkElement;

           if (elem != null)
           {

               Window window = null;
               if (elem is Window)
               {
                   window = (Window)elem;
               }
               else
               {
                  window = elem.Parent as Window;
               }
              window.DialogResult = true;
           }

             
        }

        public static void TryCancel(this ViewModelBase viewModel)
        {
            var elem = viewModel.View as FrameworkElement;

            if (elem != null)
            {
                Window window = null;
                if (elem is Window)
                {
                    window = (Window)elem;
                }
                else
                {
                    window = elem.Parent as Window;
                }
                window.DialogResult = false;
            }
        }


    }
}
