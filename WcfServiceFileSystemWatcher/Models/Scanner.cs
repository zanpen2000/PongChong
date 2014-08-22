using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace WcfServiceFileSystemWatcher.Models
{
    internal class Scanner
    {
        //public static List<string> ScanDirectory(string dirname)
        //{
        //    List<string> files = new List<string>();

        //    var allfiles = System.IO.Directory.GetFiles(dirname, "*", System.IO.SearchOption.AllDirectories);

        //    files.AddRange(allfiles.TakeWhile(f => { return System.IO.File.Exists(f); }));

        //    return files;
        //}

        public static List<ScannedFilesModel> ScanDirectory(string dirname)
        {
            var allfiles = System.IO.Directory.GetFiles(dirname, "*", System.IO.SearchOption.AllDirectories).ToList();

            List<ScannedFilesModel> lst = new List<ScannedFilesModel>();

            Parallel.ForEach(allfiles, (file) =>
            {
                string hash = FileHash.Calc(file);
                var model = new ScannedFilesModel()
                {
                    filehash = hash,
                    got = false,
                    fullpath = file,
                    gottime = DateTime.Now.ToString(),
                    rootpath = dirname,
                    scantime = DateTime.Now.ToString()
                };
                lst.Add(model);
            });
            return lst;
        }
    }
}
