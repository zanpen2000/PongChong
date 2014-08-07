using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FilesPuppy.Aop
{
    public class LogAttribute : AOPAttribute
    {
        public static ILogger logger = new StreamLoggerFactory().Create(@"appendFiles");

        public override void Execute(params object[] objs)
        {
            if (objs != null && objs.Count() > 0)
            {
                var now = DateTime.Now.ToShortDateString();

                var eParam = (objs[0] as Models.WatcherProcess).eParam;

                if (eParam.GetType() == typeof(RenamedEventArgs))
                {
                    var arg = eParam as RenamedEventArgs;
                    logger.InfoFormat("{0} [Renamed] {1} -> {2}", now, arg.OldFullPath, arg.FullPath);
                }
                else
                {
                    FileSystemEventArgs e = (FileSystemEventArgs)eParam;
                    if (e.ChangeType == WatcherChangeTypes.Created)
                    {
                        logger.InfoFormat("{0} [Created] {1}", now, e.FullPath);
                    }
                    else if (e.ChangeType == WatcherChangeTypes.Changed)
                    {
                        logger.InfoFormat("{0} [Changed] {1}", now, e.FullPath);
                    }
                    else if (e.ChangeType == WatcherChangeTypes.Deleted)
                    {
                        logger.InfoFormat("{0} [Deleted] {1}", now, e.FullPath);
                    }
                    else
                    {
                        logger.InfoFormat("{0} [Unknown] {1}", now, e.FullPath);
                    }
                }
            }
        }
    }
}
