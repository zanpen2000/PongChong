using System;
using System.Collections.Generic;
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

        public IEnumerable<string> GetLastFilesByDirectory(string root)
        {
            return access.GetLastFiles(root);
        }

        public bool DeleteFile(string file)
        {
            return access.DeleteFile(file);
        }

        public bool AddFile(string root, string filepath)
        {
            return access.AppendFile(root, filepath);
        }

        public int AddFiles(string root, IEnumerable<string> filepaths)
        {
            return access.AppendFiles(root, filepaths);
        }

        public bool InsertGetFileTimeLog(string root)
        {
            return access.InsertGetFileTimeLog(root);
        }

        public void ScanDirectory(string p)
        {
            var files = Scanner.ScanDirectory(p);
            access.AppendFiles(p, files);
        }

        public void ScanDirectorys(IEnumerable<string> dirs)
        {
            foreach (var dir in dirs)
            {
                this.ScanDirectory(dir);
            }   
        }

        public List<string> GetFiles(string dirname)
        {
            return Scanner.ScanDirectory(dirname);
        }
    }
}
