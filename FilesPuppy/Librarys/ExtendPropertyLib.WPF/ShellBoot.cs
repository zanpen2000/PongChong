using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Reflection;
using ExtendPropertyLib.WPF;
using ExtendPropertyLib;
using System.Windows;
using System.IO;
//using MaxZhang.EasyEntities.Dynamic.Aop;

namespace ExtendPropertyLib.WPF
{
    public class ShellBooter
    {



        public ShellBooter()
        {
         
            PrepareApplication();
            Config();
         
        }

        /// <summary>
        /// Provides an opportunity to hook into the application object.
        /// </summary>
        protected virtual void PrepareApplication()
        {
            Application.Current.Startup += OnStartup;
            
            Application.Current.Exit += OnExit;

         


        }

        public bool IsProxy { set; get; }

        public static CompositionContainer container;


        private void Config()
        {
            try
            {
                var catalog = new AggregateCatalog(new AssemblyCatalog(Assembly.GetEntryAssembly())
                   , new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory));
                //string addinsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Addins");
                //if(Directory.Exists(addinsPath))
                    //catalog.Catalogs.Add(new DirectoryCatalog(addinsPath));


                container = new CompositionContainer(catalog);
                var batch = new CompositionBatch();

                batch.AddExportedValue<IWindowManager>(new WindowManager());
                container.Compose(batch);

                BuildUp(this);
            }
            catch { }
        }


        /// <summary>
        /// 插件接口加载到系统中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m"></param>
        public virtual void Compose<T>(T m)
        {
            var batch = new CompositionBatch();

            batch.AddExportedValue<T>(m);
            container.Compose(batch);

        }



        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="instance">The instance to perform injection on.</param>
        public virtual void BuildUp(object instance) {
          
            container.SatisfyImportsOnce(instance);
        }



        /// <summary>
        /// Override this to add custom behavior to execute after the application starts.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The args.</param>
        protected virtual void OnStartup(object sender, StartupEventArgs e) {
            try
            {
                DisplayRootView();
            }
            catch { }
        }

        /// <summary>
        /// Override this to add custom behavior on exit.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        protected virtual void OnExit(object sender, EventArgs e) { 
        
        
        }


        public void DisplayRootView()
        {
            try{
                var shell = container.GetExport<IShell>().Value;

                var obj = shell as ViewModelBase;
                //if (IsProxy)
                //    obj = DynamicProxy.CreateProxy<ViewModelBase>(obj);

                 obj.OnDoCreate(obj, null);

                BuildUp(obj);
           
                IWindowManager wm = container.GetExport<IWindowManager>().Value;
                wm.Show(obj);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"错误");
            }
            //IWindowManager wm = new WindowManager();
            //wm.Show(shell);
        }
        

    }
}
