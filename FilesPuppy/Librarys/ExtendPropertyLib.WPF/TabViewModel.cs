/******************************************************************************
 *  作者：       Maxzhang1985
 *  创建时间：   2012/4/18 10:14:36
 *
 *
 ******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;

namespace ExtendPropertyLib.WPF
{
    /// <summary>
    /// 用于Tab页的ViewModel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabViewModel<T> : ViewModelBase<T> where T : ExtendObject
    {
      
        public override void OnLoad()
        {
            InitView();
        }

        protected Selector TheSelector;
        protected TabItem ThisTabItem;

        private void InitView()
        {
            var view = this.View as FrameworkElement;
            TheSelector = VisualTreeHelperEx.GetParentElement<Selector>(view);
            ThisTabItem = VisualTreeHelperEx.GetParentElement<TabItem>(view);
        }

        /// <summary>
        /// 设置当前选择项
        /// </summary>
        public bool IsSelected {
            set
            {
                if(value == true)
                    TheSelector.SelectedItem = ThisTabItem;
            }

            get { 
                return TheSelector.SelectedItem == this.ThisTabItem; 
            } 
        }
        
      




    }
}
