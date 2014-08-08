using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesPuppy.Models
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

        public static List<string> ScanDirectory(string dirname)
        {
            List<string> files = new List<string>();

            var allfiles = System.IO.Directory.GetFiles("", "", System.IO.SearchOption.AllDirectories);

            files.AddRange(allfiles.TakeWhile(f => { return System.IO.File.Exists(f); }));

            return files;
        }
    }
}
