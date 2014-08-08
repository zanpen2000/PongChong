using FilesPuppy.Layers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FilesPuppy.Aop
{
    public class AccessLogAttribute : AOPAttribute
    {

        public override void Execute(params object[] objs)
        {

            string filename = string.Empty;

            if (objs != null && objs.Count() > 0)
            {
                var now = DateTime.Now.ToShortDateString();

                var eParam = (objs[0] as Models.WatcherProcess).eParam;

                if (eParam.GetType() == typeof(RenamedEventArgs))
                {
                    var arg = eParam as RenamedEventArgs;

                    filename = arg.FullPath;
                }
                else
                {
                    FileSystemEventArgs e = (FileSystemEventArgs)eParam;
                    if (e.ChangeType == WatcherChangeTypes.Created)
                    {
                        filename = e.FullPath;

                        if (File.Exists(filename))
                        {
                            ServiceCaller.ServiceExecute<WcfServiceFileSystemWatcher.IWatcher>((w) =>
                            {
                                w.AddFile("", filename);
                            });
                        }

                    }
                    else if (e.ChangeType == WatcherChangeTypes.Changed)
                    {
                        filename = e.FullPath;

                        if (File.Exists(filename))
                        {
                            ServiceCaller.ServiceExecute<WcfServiceFileSystemWatcher.IWatcher>((w) =>
                            {
                                w.AddFile("", filename);
                            });
                        }
                    }
                    else if (e.ChangeType == WatcherChangeTypes.Deleted)
                    {
                        filename = e.FullPath;

                        if (File.Exists(filename))
                        {
                            ServiceCaller.ServiceExecute<WcfServiceFileSystemWatcher.IWatcher>((w) =>
                            {
                                w.DeleteFile(filename);
                            });
                        }
                    }
                    else
                    {
                        filename = e.FullPath;
                    }
                }

            }
        }


    }
}
