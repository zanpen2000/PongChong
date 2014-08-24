using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WcfServiceFileSystemWatcher.Models
{
    /// <summary>
    /// 本地扫描包装
    /// </summary>
    public class ScannerHelper
    {
        public static async Task<string> ScanDirectoryAsync(string dir)
        {
            return await Task.Factory.StartNew(() =>
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(Scanner.ScanDirectory(dir));
            });
        }

        public static string ScanDirectory(string dir)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(Scanner.ScanDirectory(dir));
        }

        public static async Task<bool> ScanDirectory(string dir, Func<string, bool> callback)
        {
            return await Task.Factory.StartNew(() =>
             {
                 var result = Newtonsoft.Json.JsonConvert.SerializeObject(Scanner.ScanDirectory(dir));
                 return callback(result);
             });
        }
    }
}
