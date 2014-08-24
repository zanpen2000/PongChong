using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceFileSystemWatcher.Models
{
    public class Scanner
    {
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
                    fullpath = file,
                    rootpath = dirname,
                };
                lst.Add(model);
            });
            return lst;
        }
    }
}
