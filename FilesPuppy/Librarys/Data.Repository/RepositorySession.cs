using MaxZhang.EasyEntities.Persistence.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Repository
{
    public class RepositorySession : IDisposable
    {
        private readonly IDataProvider _provider = null;
        private Repository _repository = null;

        public RepositorySession(IDataProvider provider,Repository rep)
        {
            _provider = provider;
            _repository = rep;
        }

        public IDataProvider DataProvider { get { return _provider; } }

        public Repository Factory { get{  return _repository; }}

        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}
