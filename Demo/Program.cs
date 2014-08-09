using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo
{
    using IronPython.Hosting;
    using Microsoft.Scripting.Hosting;

    class Program
    {
        static void Main(string[] args)
        {
            var now = DateTime.Now;
            Console.WriteLine(now.ToString());
            var s = ScanDirectory("G:\\SourceCode");

            var over = DateTime.Now;

            Console.WriteLine(over.ToString());
            Console.WriteLine(s.Count.ToString());

            Console.ReadKey();
        }

        public static List<string> ScanDirectory(string dirname)
        {
            List<string> files = new List<string>();

            var allfiles = System.IO.Directory.GetFiles(dirname, "*", System.IO.SearchOption.AllDirectories);

            files.AddRange(allfiles.TakeWhile(f => { return System.IO.File.Exists(f); }));

            return files;
        }
    }
}
