using MaxZhang.EasyEntities.Persistence;
using MaxZhang.EasyEntities.Persistence.Linq;
using MaxZhang.EasyEntities.Persistence.Mapping;
using MaxZhang.EasyEntities.Persistence.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Data.Repository
{
    internal class DataRepository<TModel> : IRepository<TModel>, IDisposable where TModel : DbObject
    {
        protected IDataProvider _provider = null;
        protected DbSession _dbSession = null;
        private IModelStateChangedProvider _stateChangedProvider = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="stateChangedProvider">改变数据实体的状态算法提供者</param>
        public DataRepository(IDataProvider provider, IModelStateChangedProvider stateChangedProvider = null)
        {
            this._provider = provider;
            this._stateChangedProvider = stateChangedProvider;
            _dbSession = new DbSession(this._provider);
        }


        public bool Add(TModel model)
        {
            if (_stateChangedProvider != null)
                _stateChangedProvider.StateChange(model, this._dbSession);

            _dbSession.InsertTransaction(model);
            _dbSession.SubmitChanges();
            return true;
        }

        public bool AddRange(IEnumerable<TModel> modelList)
        {
            var list = modelList.ToList();
            foreach (var model in list)
            {
                if (_stateChangedProvider != null)
                    _stateChangedProvider.StateChange(model, this._dbSession);
            }

            _dbSession.InsertAllTransaction(list);
            _dbSession.SubmitChanges();
            return true;
        }

      
        public bool Update(TModel model)
        {
            _dbSession.UpdateTransaction(model);
            _dbSession.SubmitChanges();
            return true;
        }
        public IOperatorWhere<TModel> Update(Expression<Func<TModel, object>> objExp)
        {
            return _dbSession.Update(objExp);
        }
        public List<TModel> Get(Expression<Func<TModel, bool>> whereExpression = null)
        {
            if (whereExpression != null)
                return this._dbSession.CreateQuery<TModel>().Where(whereExpression).ToList();
            else
                return this._dbSession.CreateQuery<TModel>().ToList();
            
        }
        public SelectQuery<TModel> CreateQuery()
        {
            return _dbSession.CreateQuery<TModel>();
        }
        public LinqQuery<TModel> CreateLinq()
        {
            return _dbSession.CreateLinq<TModel>();
        }
        public bool Remove(TModel model)
        {
            _dbSession.RemoveTransaction(model);
            _dbSession.SubmitChanges();
            return true;
        }
        public IOperatorWhere<TModel> Remove()
        {
            return _dbSession.Remove<TModel>();
        }
        public IOperatorWhere<TModel> Delete()
        {

            return _dbSession.Delete<TModel>();
        }
        public void Dispose()
        {
            Close();
        }
        public void Close()
        {
            this._dbSession.Dispose();
        }
        public string Log
        {
            get { return this._dbSession.Log; }
        }
         }
}
