using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfServiceFileSystemWatcher
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    public class Watcher : IWatcher
    {
        private database.AccessDomain access = new database.AccessDomain();

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

        /// <summary>
        /// 获取最后更新的一批文件，并记录时间戳
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetLastFiles()
        {
            return access.GetLastFiles();
        }

        public bool AddFile(string filepath)
        {
            return access.AppendFile(filepath);
        }

        public bool AddFiles(IEnumerable<string> filepaths)
        {
            return access.AppendFiles(filepaths);
        }

        public bool InsertGetFileTimeLog()
        {
            return access.InsertGetFileTimeLog();
        }
    }
}
