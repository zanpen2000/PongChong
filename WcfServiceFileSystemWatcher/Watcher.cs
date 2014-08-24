using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfServiceFileSystemWatcher.Models;

namespace WcfServiceFileSystemWatcher
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    public class Watcher : IWatcher
    {

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }


        public async System.Threading.Tasks.Task<string> GetJsonString(string dir)
        {
            return await ScannerHelper.ScanDirectoryAsync(dir);
        }


        public System.IO.Stream DownloadFile(string filepath)
        {

            if (!File.Exists(filepath))//判断文件是否存在
            {
                return null;
            }

            try
            {
                Stream myStream = File.OpenRead(filepath);
                return myStream;
            }
            catch { return null; }
        }
    }
}
