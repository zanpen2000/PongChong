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

        public int AppendFiles(string root, IEnumerable<string> files)
        {
            string insql = @"insert into scanedfiles(rootpath,fullpath,scantime,got) values(@root,@path,@stime,@got)";

            List<OleDbParameter[]> paras = new List<OleDbParameter[]>();

            foreach (string file in files)
            {
                if (FileExists(file)) continue;// 文件存在就不添加了

                OleDbParameter[] parameters = new OleDbParameter[4] 
                { 
                    new OleDbParameter("@root",root),
                    new OleDbParameter("@path",file),
                    new OleDbParameter("@stime",DateTime.Now.ToString()),
                    new OleDbParameter("@got",false)
                };

                paras.Add(parameters);
            }

            return oledb.OleDbExecuteMany(insql, paras);
        }

        public bool DeleteFile(string file)
        {
            if (!FileExists(file)) return true;

            string sql = "delete from scanedfiles where fullpath='" + file + "'";

            return oledb.OleDbExecute(sql);
        }


        public bool AppendFile(string root, string file)
        {
            if (FileExists(file)) return true;// 文件存在就不添加了

            string insql = @"insert into scanedfiles(rootpath, fullpath,scantime,got) values(@root,@path,@stime,@got)";

            OleDbParameter[] parameters = new OleDbParameter[4] 
                { 
                new OleDbParameter("@root",root),
                new OleDbParameter("@path",file),
                new OleDbParameter("@stime",DateTime.Now.ToString()),
                new OleDbParameter("@got",false)
                
                };

            return oledb.OleDbExecute(insql, parameters);

        }

        public bool FileExists(string filename)
        {
            DataSet ds;
            if (oledb.GetDataSet("select * from scanedfiles where fullpath='" + filename + "'", out ds))
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

        public bool InsertGetFileTimeLog(string root)
        {
            string insql = @"insert into GetFileTimeLog(getTime,rootpath) values(@gtime,@root)";

            OleDbParameter[] parameters = new OleDbParameter[2] 
                { 
                new OleDbParameter("@root",root),
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

        public IEnumerable<string> GetLastFiles(string root)
        {
            List<string> files = new List<string>();
            DataSet ds;
            if (oledb.GetDataSet("select * from ScanedFiles where got = false and rootpath='" + root + "'", out ds))
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

        internal bool AppendFiles(string root, List<Models.ScannedFilesModel> files)
        {
            bool result = false;

            string insql = @"insert into scanedfiles(rootpath,filehash,fullpath,scantime,got) values(@root,@filehash,@path,@stime,@got)";

            foreach (var file in files)
            {
                if (FileExists(file.fullpath))
                {
                    UpdateFile(file);
                }

                OleDbParameter[] parameters = new OleDbParameter[5] 
                { 
                    new OleDbParameter("@root",file.rootpath),
                    new OleDbParameter("@filehash",file.filehash),
                    new OleDbParameter("@path",file.fullpath),
                    new OleDbParameter("@stime",file.scantime),
                    new OleDbParameter("@got",false)
                
                };

                result &= oledb.OleDbExecute(insql, parameters);
            }

            return result;
        }

        public bool UpdateFile(Models.ScannedFilesModel file)
        {
            string update = "update scanedfiles  set filehash=" + file.filehash + ", `got`= " + file.got + " ,`gottime`='" + file.gottime + "' where `fullpath`='" + file.fullpath + "'";
            if (!oledb.OleDbExecute(update))
                return false;
            return true;
        }
    }
}
