using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfWcfServer.Models
{
    public class DirListHelper
    {
        private static string _dirFilename = "dirs.config";

        public static IEnumerable<string> Load()
        {
            if (System.IO.File.Exists(_dirFilename))
            {
                return System.IO.File.ReadLines(_dirFilename);
            }
            else return null;
        }

        public static void Save(IEnumerable<string> _dirlist)
        {
            System.IO.File.WriteAllLines(_dirFilename, _dirlist);
        }
    }
}
