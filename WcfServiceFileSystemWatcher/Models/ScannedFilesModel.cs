using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceFileSystemWatcher.Models
{
    public class ScannedFilesModel
    {
        public int id { get; set; }
        public string fullpath { get; set; }
        public string scantime { get; set; }
        public bool got { get; set; }
        public string gottime { get; set; }
    }
}
