using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceFileSystemWatcher.Models
{
    public class ScannedFilesModel
    {
        public string rootpath { get; set; }

        public string filehash { get; set; }
        public string fullpath { get; set; }
        public string scantime { get; set; }
        public bool got { get; set; }
        public string gottime { get; set; }
    }
}
