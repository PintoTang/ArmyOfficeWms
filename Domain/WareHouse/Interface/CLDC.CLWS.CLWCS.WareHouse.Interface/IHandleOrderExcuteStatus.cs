using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Interface
{
   public interface IHandleOrderExcuteStatus
    {
        /// <summary>
        /// 用于处理指令变化
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="order"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        OperateResult HandleOrderChange(DeviceName deviceName, ExOrder order, OrderExcuteStatusEnum type);

    }
}
