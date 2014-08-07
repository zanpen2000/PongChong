using Castle.Core.Logging;
using FilesPuppy.Aop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesPuppy.Models
{
    public delegate void Completed(string key);

    public class WatcherProcess
    {
        private object sender;
        public object eParam;

        public event RenamedEventHandler OnRenamed = delegate { };
        public event FileSystemEventHandler OnChanged = delegate { };
        public event FileSystemEventHandler OnCreated = delegate { };
        public event FileSystemEventHandler OnDeleted = delegate { };
        public event Completed OnCompleted = delegate { };

        public WatcherProcess(object sender, object eParam)
        {
            this.sender = sender;
            this.eParam = eParam;
        }

        public WatcherProcess()
        {

        }

        [Log]
        public virtual void Process()
        {
            if (eParam.GetType() == typeof(RenamedEventArgs))
            {
                OnRenamed.Invoke(sender, (RenamedEventArgs)eParam);
                OnCompleted.Invoke(((RenamedEventArgs)eParam).FullPath);
            }
            else
            {
                FileSystemEventArgs e = (FileSystemEventArgs)eParam;
                if (e.ChangeType == WatcherChangeTypes.Created)
                {
                    
                    OnCreated.Invoke(sender, e);
                    OnCompleted.Invoke(e.FullPath);
                }
                else if (e.ChangeType == WatcherChangeTypes.Changed)
                {
                    OnChanged.Invoke(sender, e);
                    OnCompleted.Invoke(e.FullPath);
                }
                else if (e.ChangeType == WatcherChangeTypes.Deleted)
                {
                    OnDeleted.Invoke(sender, e);
                    OnCompleted.Invoke(e.FullPath);
                }
                else
                {
                    OnCompleted.Invoke(e.FullPath);
                }
            }
        }
    }
}

