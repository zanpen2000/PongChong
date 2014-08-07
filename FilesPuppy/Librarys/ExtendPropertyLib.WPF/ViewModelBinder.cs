using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ExtendPropertyLib.WPF;

namespace ExtendPropertyLib
{
    /// <summary>
    /// ViewModel与View绑定器，用于对View与ViewModel进行数据和事件绑定
    /// </summary>
    public static class ViewModelBinder
    {
        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="view"></param>
        /// <param name="eventBinding"></param>
        public static void Bind(this ViewModelBase viewModel, FrameworkElement view,bool eventBinding =false)
        {
            var type = viewModel.GetType();
            viewModel.View = view;
            view.DataContext = viewModel;
            if(eventBinding)
                BindEvents(viewModel, view);
        }



        


        /// <summary>
        /// 事件绑定
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="view"></param>
        public static void BindEvents( ExtendObject viewModel, FrameworkElement view)
        {
            view.Loaded += (s, e) => viewModel.OnLoad();

            var type = viewModel.GetType();
            var methodList = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var methodNameList = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Select(m=>m.Name).ToArray();
            List<Control> controls = GetBindingControls(methodNameList, view);

            foreach (var control in controls)
            {
          
                //WeakEventManager
                var method = GetMethod(methodList,control.Name);
                object[] paramsv = null;
                if (method.GetParameters().Count() > 0)
                    paramsv = new object[] { viewModel };
                if (control is Button)
                {
                    var button = control as ButtonBase;
                  
                    button.Click += (s, e) => method.Invoke(viewModel, paramsv);
                }
                else if (control is CheckBox || control is RadioButton)
                {
                    var tb = control as ToggleButton;
                    tb.Checked += (s, e) => method.Invoke(viewModel, paramsv);
                }
                else if(control is MenuItem)
                {
                    var menu = control as MenuItem;
                    menu.Click += (s, e) => method.Invoke(viewModel, paramsv);
                }
                else if (control is HeaderedItemsControl)
                {
                    var headerControl = control as HeaderedItemsControl;
                    headerControl.MouseDown += (s, e) =>
                        {
                            method.Invoke(viewModel, paramsv);
                        };
                }
                else if (control is Selector)
                {
                    var selectorControl = control as Selector;
                    selectorControl.SelectionChanged += (s, e) => method.Invoke(viewModel, paramsv);
                }
                else if (control is TreeView)
                {
                    var treeItem = control as TreeView;
                    treeItem.SelectedItemChanged += (s, e) => method.Invoke(viewModel, paramsv);
                }
                else
                {

                }


            }


        }

        private static MethodInfo GetMethod(MethodInfo[] ms , string name)
        {
           return ms.FirstOrDefault(m => m.Name == name);
        }
        /// <summary>
        /// 查找ViewModel对象中方法名与View对象中元素名对应的控件
        /// </summary>
        /// <param name="methodNameList"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        private static List<Control> GetBindingControls(string[] methodNameList,FrameworkElement view)
        {
            List<Control> controls = new List<Control>(10);
            foreach (var mName in methodNameList)
            {
                if (char.IsUpper(mName[0]))
                {
                    var element = view.FindName(mName) as Control;
                    if(element!=null)
                        controls.Add(element);
                }
            }
            return controls;
        }


    }
}
