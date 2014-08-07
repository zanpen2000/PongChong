using Castle.DynamicProxy;
using DirectoryDog.AopInterceptor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DirectoryDog.Models
{
    public class WatchPuppy
    {
        private FileSystemWatcher fsWather;

        private Hashtable hstbWather;

        public event RenamedEventHandler OnRenamed = delegate { };
        public event FileSystemEventHandler OnChanged = delegate { };
        public event FileSystemEventHandler OnCreated = delegate { };
        public event FileSystemEventHandler OnDeleted = delegate { };

        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="path">要监控的路径</param> 
        public WatchPuppy(string path, string filter)
        {
            if (!Directory.Exists(path))
            {
                throw new Exception("找不到路径：" + path);
            }

            hstbWather = new Hashtable();

            fsWather = new FileSystemWatcher(path);
            // 是否监控子目录
            fsWather.IncludeSubdirectories = true;
            fsWather.Filter = filter;
            fsWather.Renamed += new RenamedEventHandler(fsWather_Renamed);
            fsWather.Changed += new FileSystemEventHandler(fsWather_Changed);
            fsWather.Created += new FileSystemEventHandler(fsWather_Created);
            fsWather.Deleted += new FileSystemEventHandler(fsWather_Deleted);
        }

        /// <summary> 
        /// 开始监控 
        /// </summary> 
        public void Start()
        {
            fsWather.EnableRaisingEvents = true;
        }

        /// <summary> 
        /// 停止监控 
        /// </summary> 
        public void Stop()
        {
            fsWather.EnableRaisingEvents = false;
        }

        /// <summary> 
        /// filesystemWatcher 本身的事件通知处理过程 
        /// </summary> 
        /// <param name="sender"></param> 
        /// <param name="e"></param> 
        private void fsWather_Renamed(object sender, RenamedEventArgs e)
        {
            lock (hstbWather)
            {
                hstbWather.Add(e.FullPath, e);
            }

            WatcherProcess watcherProcess = new WatcherProcess(sender, e);
            watcherProcess.OnCompleted += new Completed(WatcherProcess_OnCompleted);
            watcherProcess.OnRenamed += new RenamedEventHandler(WatcherProcess_OnRenamed);

            ProxyGenerator generator = new ProxyGenerator();
            InterceptorProxy proxy = new InterceptorProxy();
            var process = generator.CreateClassProxyWithTarget(watcherProcess, proxy);

            Thread thread = new Thread(process.Process);
            thread.Start();
        }

        private void WatcherProcess_OnRenamed(object sender, RenamedEventArgs e)
        {
            OnRenamed(sender, e);
        }

        private void fsWather_Created(object sender, FileSystemEventArgs e)
        {
            lock (hstbWather)
            {
                hstbWather.Add(e.FullPath, e);
            }


            WatcherProcess watcherProcess = new WatcherProcess(sender, e);
            watcherProcess.OnCompleted += new Completed(WatcherProcess_OnCompleted);
            watcherProcess.OnCreated += new FileSystemEventHandler(WatcherProcess_OnCreated);

            ProxyGenerator generator = new ProxyGenerator();
            InterceptorProxy proxy = new InterceptorProxy();
            var process = generator.CreateClassProxyWithTarget(watcherProcess, proxy);
            Thread threadDeal = new Thread(process.Process);
            threadDeal.Start();
        }

        private void WatcherProcess_OnCreated(object sender, FileSystemEventArgs e)
        {
            OnCreated(sender, e);
        }

        private void fsWather_Deleted(object sender, FileSystemEventArgs e)
        {
            lock (hstbWather)
            {
                if (!hstbWather.ContainsKey(e.FullPath))
                    hstbWather.Add(e.FullPath, e);
            }
            WatcherProcess watcherProcess = new WatcherProcess(sender, e);
            watcherProcess.OnCompleted += new Completed(WatcherProcess_OnCompleted);
            watcherProcess.OnDeleted += new FileSystemEventHandler(WatcherProcess_OnDeleted);

            ProxyGenerator generator = new ProxyGenerator();
            InterceptorProxy proxy = new InterceptorProxy();
            var process = generator.CreateClassProxyWithTarget(watcherProcess, proxy);

            Thread tdDeal = new Thread(process.Process);
            tdDeal.Start();
        }

        private void WatcherProcess_OnDeleted(object sender, FileSystemEventArgs e)
        {
            OnDeleted(sender, e);
        }

        private void fsWather_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                if (hstbWather.ContainsKey(e.FullPath))
                {
                    WatcherChangeTypes oldType = ((FileSystemEventArgs)hstbWather[e.FullPath]).ChangeType;
                    if (oldType == WatcherChangeTypes.Created || oldType == WatcherChangeTypes.Changed)
                    {
                        return;
                    }
                }
            }

            lock (hstbWather)
            {
                hstbWather.Add(e.FullPath, e);
            }
            WatcherProcess watcherProcess = new WatcherProcess(sender, e);
            watcherProcess.OnCompleted += new Completed(WatcherProcess_OnCompleted);
            watcherProcess.OnChanged += new FileSystemEventHandler(WatcherProcess_OnChanged);

            ProxyGenerator generator = new ProxyGenerator();
            InterceptorProxy proxy = new InterceptorProxy();
            var process = generator.CreateClassProxyWithTarget(watcherProcess, proxy);

            Thread thread = new Thread(process.Process);
            thread.Start();
        }

        private void WatcherProcess_OnChanged(object sender, FileSystemEventArgs e)
        {
            OnChanged(sender, e);
        }

        public void WatcherProcess_OnCompleted(string key)
        {
            lock (hstbWather)
            {
                hstbWather.Remove(key);
            }
        }
    }
}
