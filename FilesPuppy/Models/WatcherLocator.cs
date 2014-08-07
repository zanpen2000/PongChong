using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilesPuppy.Models
{
    public class WatcherLocator
    {
        public static List<WatchPuppy> Watchers = new List<WatchPuppy>();

        public static void AddWatcher(WatchPuppy watcher)
        {
            if(Watchers.Count(w=>w.Path == watcher.Path)<1)
            {
                Watchers.Add(watcher);
            }
        }

        public static void AddWatcher(string dir, string filter = "*")
        {
            if (Watchers.Count(w => w.Path == dir) < 1)
            {
                WatchPuppy p = new WatchPuppy(dir, filter);
                Watchers.Add(p);
            }
        }

        public static WatchPuppy GetWatcher(string dir)
        {
            return Watchers.Find(w => w.Path == dir);
        }
    }
}
