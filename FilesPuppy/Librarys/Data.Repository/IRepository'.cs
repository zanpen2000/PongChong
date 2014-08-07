using MaxZhang.EasyEntities.Persistence;
using MaxZhang.EasyEntities.Persistence.Linq;
using MaxZhang.EasyEntities.Persistence.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Data.Repository
{
    public interface IRepository<TModel> : IRepository, IDisposable where TModel : DbObject
    {
        bool Add(TModel model);
        bool AddRange(IEnumerable<TModel> modelList);
        bool Remove(TModel model);
        IOperatorWhere<TModel> Remove();
        bool Update(TModel model);
        IOperatorWhere<TModel> Update(Expression<Func<TModel, object>> objExp);
        List<TModel> Get(Expression<Func<TModel, bool>> whereExpression);
        SelectQuery<TModel> CreateQuery();
        LinqQuery<TModel> CreateLinq();
        IOperatorWhere<TModel> Delete();

        string Log { get; }

        void Close();

    }
}
