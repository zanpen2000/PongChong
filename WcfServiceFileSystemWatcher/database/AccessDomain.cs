using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace WcfServiceFileSystemWatcher.database
{
    public class AccessDomain : DbHelper
    {
        public AccessDomain()
        {
            oledb.Url = PublicUdl.connMdbUDL;
        }

        public bool AppendFiles(IEnumerable<string> files)
        {
            bool result = false;

            string insql = @"insert into scanedfiles(fullpath,scantime,got) values(@path,@stime,@got)";

            foreach (string file in files)
            {
                if (FileExists(file)) continue;// 文件存在就不添加了

                OleDbParameter[] parameters = new OleDbParameter[3] 
                { 
                new OleDbParameter("@path",file),
                new OleDbParameter("@stime",DateTime.Now.ToString()),
                new OleDbParameter("@got",false)
                
                };

                result &= oledb.OleDbExecute(insql, parameters);
            }

            return result;
        }

        public bool DeleteFile(string file)
        {
            if (!FileExists(file)) return true;

            string sql = "delete from scanedfiles where fullpath='"+file+"'";

            return oledb.OleDbExecute(sql);
        }


        public bool AppendFile(string file)
        {
            if (FileExists(file)) return true;// 文件存在就不添加了

            string insql = @"insert into scanedfiles(fullpath,scantime,got) values(@path,@stime,@got)";

            OleDbParameter[] parameters = new OleDbParameter[3] 
                { 
                new OleDbParameter("@path",file),
                new OleDbParameter("@stime",DateTime.Now.ToString()),
                new OleDbParameter("@got",false)
                
                };

            return oledb.OleDbExecute(insql, parameters);

        }

        public bool FileExists(string filename)
        {
            DataSet ds;
            if (oledb.GetDataSet("select * from scanedfiles where fullpath='"+filename+"'", out ds))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    return true;
            }

            return false;
        }

        public bool UpdateFile(string file)
        {
            string update = "update scanedfiles  set `got`= true ,`gottime`='" + DateTime.Now + "' where `fullpath`='" + file + "'";
            if (!oledb.OleDbExecute(update))
                return false;
            return true;
        }

        public bool InsertGetFileTimeLog()
        {
            string insql = @"insert into GetFileTimeLog(getTime) values(@gtime)";

            OleDbParameter[] parameters = new OleDbParameter[1] 
                { 
                new OleDbParameter("@gtime",DateTime.Now.ToString())
                
                };

            if (!oledb.OleDbExecute(insql))
                return false;
            return true;
        }

        public IEnumerable<string> GetLastFiles()
        {
            List<string> files = new List<string>();
            DataSet ds;
            if (oledb.GetDataSet("select * from ScanedFiles where got = false", out ds))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        files.Add(ds.Tables[0].Rows[i]["fullpath"].ToString());
                    }
                }
            }
            return files;
        }
    }
}
