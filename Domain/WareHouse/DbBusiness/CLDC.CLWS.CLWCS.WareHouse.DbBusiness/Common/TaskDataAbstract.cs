using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common
{
    public abstract class TaskDataAbstract : DatabaseBusinessAbstract<TaskModel>
    {
        protected string WareHouseCode = SystemConfig.Instance.WhCode;
        public TaskDataAbstract(IDbHelper dbHelper) : base(dbHelper)
        {
        }
    }
}
