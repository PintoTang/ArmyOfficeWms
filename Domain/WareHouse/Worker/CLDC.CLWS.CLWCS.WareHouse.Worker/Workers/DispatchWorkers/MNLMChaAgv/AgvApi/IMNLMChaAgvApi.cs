using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.MNLMChaAgv.AgvApi
{
    public interface IMNLMChaAgvApi
    {
        /// <summary>
        /// 执行结果通知
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        OperateResult NotifyExeResult(MNLMExeResultMode cmd);
        /// <summary>
        /// 车辆状态信息上报
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        OperateResult NotifyDeviceStatus(MNLMNotifyDeviceStatusMode cmd);
        /// <summary>
        /// 车辆运行数据上报
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        OperateResult NotifyDeviceInfo(MNLMNotifyDeviceInfoMode cmd);
    }
}
