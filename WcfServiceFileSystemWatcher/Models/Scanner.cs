using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfServiceFileSystemWatcher.Models
{
    internal class Scanner
    {
        public static List<string> ScanDirectory(string dirname)
        {
            List<string> files = new List<string>();

            var allfiles = System.IO.Directory.GetFiles("", "", System.IO.SearchOption.AllDirectories);

            files.AddRange(allfiles.TakeWhile(f => { return System.IO.File.Exists(f); }));

            return files;
        }
    }
}
