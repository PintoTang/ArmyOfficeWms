using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common
{
    public abstract class TaskOrderDataAbstract : DatabaseBusinessAbstract<TaskOrderDataModel>
    {
        protected string WareHouseCode = SystemConfig.Instance.WhCode;
        protected TaskOrderDataAbstract(IDbHelper dbHelper) : base(dbHelper)
        {
        }

        /// <summary>
        /// 通过指令编号获取关联的任务编号
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public abstract OperateResult<TaskOrderDataModel> GetTaskCodeByOrderId(int orderId);

    }
}
