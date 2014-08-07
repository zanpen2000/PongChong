using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace WcfServiceFileSystemWatcher.database
{
    public static class PublicUdl
    {
        public static string connMdbUDL = "Provider=" + AppSettings.Get("Provider") + ";Data Source=" + AppSettings.Get("DataSource");
    }
}
