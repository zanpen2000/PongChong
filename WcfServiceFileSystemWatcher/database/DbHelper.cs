using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace WcfServiceFileSystemWatcher.database
{
    public class DbHelper
    {
        protected OleDbHelper oledb = new OleDbHelper();

        /// <summary>
        /// 设置Url
        /// </summary>
        /// <param name="url"></param>
        public void SetUrl(string url)
        {
            oledb.Url = url;
        }
    }
}
