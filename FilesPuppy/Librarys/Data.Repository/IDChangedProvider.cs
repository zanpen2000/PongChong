using MaxZhang.EasyEntities.Persistence;
using MaxZhang.EasyEntities.Persistence.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Data.Repository
{
    public class IDChangedProvider : IModelStateChangedProvider
    {
        private static readonly object lockObject = new object();

        public void StateChange(DbObject obj, DbSession db)
        {
            
            var tableName = (string)obj.GetValue(DbObject.TableNameProperty);
            int lenght = (int)DbMetaDataManager.GetMetaDatas(obj.GetType()).First(md => md.FieldName == "id").ColumnLength;

            IDataParameter para3 = db.Provider.CreateDataParameter("@systemValue", "", ParameterDirection.Output);
            para3.DbType = DbType.AnsiString;
            IDataParameter[] parameters = new IDataParameter[]{
                    db.Provider.CreateDataParameter("systemName",tableName),
                    db.Provider.CreateDataParameter("systemLength",lenght),
                    para3
            };
            string newID = string.Empty;
            lock (lockObject)
            {
                db.RunStoredProcedure("GetSysMaxID", parameters);
                newID = para3.Value.ToString();
            }
            dynamic dynamicObj = obj;
            dynamicObj.id = newID;
            //Changed model id property of value
        }
    }
}
