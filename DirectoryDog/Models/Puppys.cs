using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDog.Models
{
    public class WatcherLocator
    {
        public static Dictionary<string, WatchPuppy> Watchers = new Dictionary<string, WatchPuppy>();

        public static void AddWatcher(string dir, string filter = "*")
        {
            WatchPuppy p = new WatchPuppy(dir, filter);
            Watchers.Add(dir, p);
        }

        public static WatchPuppy GetWatcher(string dir)
        {
            return Watchers[dir];
        }
    }
}
