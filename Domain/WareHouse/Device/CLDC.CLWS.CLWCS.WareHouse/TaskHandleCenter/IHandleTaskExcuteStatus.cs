using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.TaskHandleCenter
{
   public interface IHandleTaskExcuteStatus<TTask>where TTask:IDeviceTaskContent
    {
       /// <summary>
       /// 用于处理指令变化
       /// </summary>
       /// <param name="task"></param>
       /// <param name="excuteStepStatus"></param>
       /// <param name="finishDevice"></param>
       /// <returns></returns>
       OperateResult HandleTaskExcuteStatus(DeviceBaseAbstract finishDevice, TTask task, TaskExcuteStepStatusEnum excuteStepStatus);

       /// <summary>
       /// 名称
       /// </summary>
       string Name { get; set; }

    }
}
