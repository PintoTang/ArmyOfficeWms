using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common
{
    public abstract class StringCharTaskAbstract : DatabaseBusinessAbstract<StringCharTaskModel>
    {
        protected string WareHouseCode = SystemConfig.Instance.WhCode;
        public StringCharTaskAbstract(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }
        /// <summary>
        /// 根据设备编号获取未完成的任务
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public abstract OperateResult<List<StringCharTaskModel>> GetUnFinishTask(int deviceId);
        /// <summary>
        /// 通过设备编号及状态获取任务
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="statusList"></param>
        /// <returns></returns>
        public abstract OperateResult<List<StringCharTaskModel>> GetTaskByStatus(int deviceId,
            List<int> statusList);
    }
}
