using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.StringCharTaskHandle
{
    /// <summary>
    /// 处理String类型任务的状态
    /// </summary>
    public interface IHandleStringCharTaskStatus
    {
        /// <summary>
        /// 处理String类型任务的完成
        /// </summary>
        /// <param name="device"></param>
        /// <param name="taskValue"></param>
        /// <returns></returns>
        OperateResult FinishStringCharTask(DeviceBaseAbstract device, StringCharTask taskValue);
    }
}
