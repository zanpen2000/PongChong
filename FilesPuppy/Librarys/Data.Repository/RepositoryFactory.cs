
using MaxZhang.EasyEntities.Persistence.Mapping;
using MaxZhang.EasyEntities.Persistence.Provider;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MaxZhang.EasyEntities.Persistence;


namespace Data.Repository
{
    public class Repository
    {

        private Repository() { }
        private static volatile Repository _factory = new Repository();
        private static object lockObject = new object();
        public static Repository Factory
        {
            get
            {
                lock (lockObject)
                {
                    if (_factory == null)
                    {
                        _factory = new Repository();
                    }
                }
                return _factory;
            }
        }


        private static IDataProvider GetProviderFromConfigFile()
        {
            IDataProvider provider = null;
            string xmlurl = string.Empty;;
            if(System.IO.Directory.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin")))
                xmlurl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "DataBaseConfig.xml");
            else
                xmlurl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataBaseConfig.xml");
            XmlHelper xmlHelper = new XmlHelper(xmlurl);
            string databaseType = xmlHelper.GetValue(@"DataBase");
            string connstring = xmlHelper.GetValue(@"ConnectionString");
            switch (databaseType.ToUpper())
            {
                case "SQLSERVER": provider = new SqlProvider(connstring); break;
                case "ACCESS": provider = new AccessProvider(connstring); break;
                case "ORACLE": provider = new OracleProvider(connstring); break;
            }
            return provider;
        }

        public static RepositorySession OpenSession()
        {
            var provider = GetProviderFromConfigFile();
            return new RepositorySession(provider, Repository.Factory);
        }

        public static DbSession CreateDbSession()
        {
            var provider = GetProviderFromConfigFile();
            return new DbSession(provider);
        }

        public IRepository<TModel> NewRepository<TModel>(bool defaultid = true) where TModel : DbObject
        {
              IModelStateChangedProvider idProvider = null;
            if (defaultid)
                idProvider = new IDChangedProvider();

            return new DataRepository<TModel>(GetProviderFromConfigFile(), idProvider);
        }

        //public TRepository NewRepository<TRepository>() where TRepository : IRepository
        //{
        //    return default(TRepository);
        //}

        public IRepositoryQuery NewRepository()
        {
            return new DataQuery(GetProviderFromConfigFile());
        }
    }
}
