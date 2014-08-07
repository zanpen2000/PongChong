/*
 * 作者：吴昊
 * 时间：2014/1/18
 * 作用：通过语句执行的数据库操作类
 * 作用域：Store.Implement
 */

using MaxZhang.EasyEntities.Persistence;
using MaxZhang.EasyEntities.Persistence.Provider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Data.Repository
{
    public class DataQuery : IRepositoryQuery
    {
        protected IDataProvider _provider = null;
        protected DbSession _dbSession = null;


        public DataQuery(IDataProvider provider)
        {
            this._provider = provider;
            _dbSession = new DbSession(this._provider);
        }

        public bool Execute(string commandText)
        {
            try
            {
                _dbSession.Execute(new Command(commandText));
                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public object ExcuteScalar(string commandText)
        {
            return _dbSession.ScalarQuery(new Command(commandText));
        }

        public System.Data.DataSet GetDataSet(string commandText)
        {
            return this._dbSession.DataSetQuery(new Command(commandText));
        }

        public System.Data.DataTable GetDataTable(string commandText)
        {
            var tables = this._dbSession.DataSetQuery(new Command(commandText)).Tables;
            return tables.Count != 0 ? tables[0] : null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public System.Data.DataSet RunStoredProcedure(string storedProcName, Parameter[] parameters)
        {
            List<IDataParameter> paras = new List<IDataParameter>();

            foreach (Parameter para in parameters)
            {
                IDataParameter p = this._dbSession.Provider.CreateDataParameter(para.Name, para.Value, para.Direction);
                p.DbType = para.Type;
                paras.Add(p);
            }
            return _dbSession.RunStoredProcedure(storedProcName, paras.ToArray());
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            this._dbSession.Dispose();
        }
    }
}
