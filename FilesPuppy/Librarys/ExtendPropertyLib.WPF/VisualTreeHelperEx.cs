/******************************************************************************
 *  作者：       Maxzhang1985
 *  创建时间：   2012/4/18 10:21:17
 *
 *
 ******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace ExtendPropertyLib.WPF
{
    class VisualTreeHelperEx
    {
        /// <summary>
        /// 得到指定类型的子节点
        /// </summary>
        /// <typeparam name="T">子节点类型</typeparam>
        /// <param name="parent">当前节点</param>
        /// <returns>子节点实例</returns>
        public static T GetChildElement<T>(DependencyObject parent) where T : FrameworkElement
        {

            T grandChild = null;

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(parent) - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T)
                    return (T)child;
                else
                {
                    grandChild = GetChildElement<T>(child);
                    if (grandChild != null)
                        return grandChild;
                }

            }
            return null;
        }

        /// <summary>
        /// 得到指定类型的父节点
        /// </summary>
        /// <typeparam name="T">父节点类型</typeparam>
        /// <param name="obj">当前节点</param>
        /// <returns>父节点实例</returns>
        public static T GetParentElement<T>(FrameworkElement obj) where T : FrameworkElement
        {
            FrameworkElement parent = obj;
            while (!(parent is T))
            {
                parent = parent.Parent as FrameworkElement;
                if (parent == null)
                    break;
            }
            return (T)parent;
        }


        /// <summary>
        /// 获得指定子节点类型的集合
        /// </summary>
        /// <typeparam name="T">子节点类型</typeparam>
        /// <param name="parent">当前节点实例</param>
        /// <returns>子节点集合实例</returns>
        public static List<T> GetChildrenElement<T>(DependencyObject parent) where T : FrameworkElement
        {
            List<T> childs = new List<T>();
            T grandChild = null;

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(parent) - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T)
                {
                    childs.Add((T)child);
                }

                var ls = GetChildrenElement<T>(child);
                if (ls.Count > 0)
                    childs.AddRange(ls);

            }


            return childs;
        }

    }
    
}
