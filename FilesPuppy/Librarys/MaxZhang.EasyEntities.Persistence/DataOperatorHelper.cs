using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using MaxZhang.EasyEntities.Persistence.Mapping;

namespace MaxZhang.EasyEntities.Persistence
{
    public static class DataOperatorHelper
    {

        /// <summary>
        /// IDataReader to model object for the 'T'.
        /// 泛型方法:将一个实现IDataReader接口的对象的所有属性通过反射，
        /// 把属性Copy到有相同字段的实体对象中.
        /// </summary>
        /// <param name="T">实体类名(Model object name)</param>
        /// <param name="reader">IDataReader接口实例对象(Object is type of IDataReader)</param>
        /// <returns>Model object for the 'T'.</returns>
        public static T ToEntity<T>(this IDataReader reader)
        {
            
            PropertyInfo[] propertys = null;
            T entity = Activator.CreateInstance<T>();
            propertys = new PropertyInfo[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
               
                    string propertyName = DbMetaDataManager.GetPropertyName(typeof(T), reader.GetName(i));
                    if (string.IsNullOrEmpty(propertyName))
                        continue;
                    propertys[i] = typeof(T).GetProperty(propertyName);
                    if (propertys[i] != null && !reader.IsDBNull(i))
                    {
                        try
                        {
                            object val = reader[i];
                            propertys[i].SetValue(entity, val, null);
                        }
                        catch (Exception exxxx)
                        {
                        }
                    }
                
            
            }

            return entity;
        }

        /// <summary>
        /// IDataReader to model object for the 'a.
        /// 泛型方法:将一个实现IDataReader接口的对象的所有属性
        /// 把属性Copy到有相同字段的匿名实体对象中.
        /// </summary>
        /// <param name="T">实体类名(Model object name)</param>
        /// <param name="reader">IDataReader接口实例对象(Object is type of IDataReader)</param>
        /// <returns>Model object for the 'a.</returns>
        public static T ToAEntity<T>(this IDataReader reader)
        {
            List<object> paramters = new List<object>();
            var t = typeof(T);
            var ps = t.GetProperties();

            foreach (PropertyInfo p in ps)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (p.Name == reader.GetName(i))
                    {
                        object val = null;
                        if (!reader.IsDBNull(i))
                            val = reader[i];
                        paramters.Add(val);
                        break;
                    }
                }

            }
           
          

            return (T)Activator.CreateInstance(typeof(T), paramters.ToArray()); ;
        }

    }
}