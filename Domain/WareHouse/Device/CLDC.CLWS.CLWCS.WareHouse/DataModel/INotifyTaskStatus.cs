using System.ComponentModel;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel
{

    /// <summary>
    /// 通知任务设备的任务状态
    /// </summary>
    public interface INotifyTaskStatus<TTask> where TTask : IUnique
    {
        /// <summary>
        /// 通知任务设备的任务状态
        /// </summary>
        /// <param name="finishDevice"></param>
        /// <param name="task"></param>
        /// <param name="taskStatus"></param>
        void UpdateTaskStatus(DeviceBaseAbstract finishDevice, TTask task, TaskExcuteStepStatusEnum taskStatus);
    }
}
