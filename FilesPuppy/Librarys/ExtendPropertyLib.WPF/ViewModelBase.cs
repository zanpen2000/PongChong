using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ExtendPropertyLib.WPF
{
    /// <summary>
    /// ViewModel对象创建工厂
    /// </summary>
    public static class ViewModelFactory
    {
        /// <summary>
        /// 创建ViewModel
        /// </summary>
        /// <typeparam name="TR"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static TR GetModel<TR>(params object[] args) where TR : ExtendObject
        {
            var m = Activator.CreateInstance(typeof(TR)) as TR;
            m.OnDoCreate(m, args);
            return m;
        }
    }



    /// <summary>
    /// ViewModel基类，没有Model
    /// </summary>
    public class ViewModelBase : ExtendObject
    {
        public override void OnDoCreate(ExtendObject item, params object[] args)
        {
            args = null;
            base.OnDoCreate(item, args);
        }

      
        /// <summary>
        /// 视图对象
        /// </summary>
        public virtual object View { set; get; }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        public virtual void Closed(){}
        /// <summary>
        /// Mef注入
        /// </summary>
        /// <param name="obj"></param>
        public virtual void BuildUp(object obj)
        {
            var boot = Application.Current.FindResource("bootstrapper") as ShellBooter;
            boot.BuildUp(obj);
        }

        // <summary>
        /// Mef插件接口加载到系统中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m"></param>
        public virtual void Compose<T>(T m)
        {
            var boot = Application.Current.FindResource("bootstrapper") as ShellBooter;
            boot.Compose(m);
        }

        /// <summary>
        /// 用于调用Dispatcher线程同步方法
        /// </summary>
        /// <param name="ac"></param>
        protected void Sync(Action ac)
        {
            var view = this.View as FrameworkElement;
            if (ac != null && view != null)
            {
                view.Dispatcher.BeginInvoke(ac);
            }
        }

        public void SetValue(ExtendProperty property, object value)
        {
            var propertyInfo = this.GetType().GetProperty(property.PropertyName);
            var attributes = (SyncAttribute[])propertyInfo.GetCustomAttributes(typeof(SyncAttribute), true );

            if (attributes.Length > 0)
            {
                Sync(() =>
                {
                    base.SetValue(property, value);
                });
            }
            else
            {
                base.SetValue(property, value);
            }
            
        }



        /// <summary>
        /// 创建子视图及ViewModel
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <returns></returns>
        public static TViewModel CreateView<TViewModel>(bool isBind=false) where TViewModel : ViewModelBase
        {
            var viewModel = ViewModelFactory.GetModel<TViewModel>();
            var viewLocator = new ViewLocator();
            var view = viewLocator.FindView(viewModel);
            if(isBind)
             ViewModelBinder.Bind(viewModel, view, true);
            viewModel.BuildUp(viewModel);
            //viewModel.OnLoad();
            return viewModel;

        }

        /// <summary>
        /// 获取或设置视图标题
        /// </summary>
        /// <returns></returns>
        public virtual string GetViewTitle()
        {
            return null;
        }

        /// <summary>
        /// 注册ViewModel命令
        /// </summary>
        /// <param name="action">方法</param>
        /// <returns></returns>
        protected virtual ICommand RegisterCommand(Action action)
        {
            return new RelayCommand(action);
        }

        protected virtual ICommand RegisterCommand(Action<object> action)
        {
            return new RelayCommand(action);
        }


        /// <summary>
        /// 注册ViewModel命令
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="canExecute">是否可执行方法</param>
        /// <returns></returns>
        protected virtual ICommand RegisterCommand(Action action, Func<bool> canExecute)
        {
            return new RelayCommand(action, canExecute);
        }

    }

    /// <summary>
    /// ViewModel基类，带有类型为T的Model对象 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ViewModelBase<T> : ViewModelBase where T : ExtendObject
    {
        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <param name="item"></param>
        /// <param name="args"></param>
        public override void OnDoCreate(ExtendObject item, params object[] args)
        {
            if (args != null && args.Count() > 0)
                Model = (T)args[0];
            else
            {
                object m = Activator.CreateInstance(typeof(T));
                if (m is ExtendObject)
                {
                    ((ExtendObject)m).OnDoCreate(null, null);
                    //((ExtendObject)m).Parent = this;
                }
                Model = (T)m;
            }
            base.OnDoCreate(item, args);
        }


     


        public static readonly ExtendProperty ModelProperty = RegisterProperty<ViewModelBase<T>>(p => p.Model, ObjectRelations.Children);
        /// <summary>
        /// ViewModel中的Model
        /// </summary>
        public virtual T Model
        {
            set { SetValue(ModelProperty, value); }
            get { return (T)GetValue(ModelProperty); }
        }


    }
}
