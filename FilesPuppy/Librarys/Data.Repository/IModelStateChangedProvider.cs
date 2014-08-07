using MaxZhang.EasyEntities.Persistence;
using MaxZhang.EasyEntities.Persistence.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Repository
{
    public interface IModelStateChangedProvider
    {
        void StateChange(DbObject obj,DbSession db);
    }
}
