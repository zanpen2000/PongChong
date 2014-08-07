using MaxZhang.EasyEntities.Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Data.Repository
{
    public interface IRepositoryQuery : IDisposable
    {
        bool Execute(string commandText);
        object ExcuteScalar(string commandText);
        DataSet GetDataSet(string commandText);
        DataTable GetDataTable(string commandText);
        DataSet RunStoredProcedure(string storedProcName, Parameter[] parameters);
    }
}
