using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.TaskHandleCenter
{
    /// <summary>
    /// 指令状态通知中心
    /// </summary>
    public interface ITaskNotifyCentre<TTask> where TTask : IDeviceTaskContent
    {
        /// <summary>
        /// 指令状态监听者列表
        /// </summary>
        Pool<IHandleTaskExcuteStatus<TTask>> TaskExcuteStatusListenerList { get; }
        /// <summary>
        /// 注册指令监听者
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        OperateResult RegisterTaskExcuteStatusListener(IHandleTaskExcuteStatus<TTask> listener);
        /// <summary>
        /// 解注册指令监听者
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        OperateResult UnRegisterTaskExcuteStatusListener(IHandleTaskExcuteStatus<TTask> listener);

        /// <summary>
        /// 通知指令变化
        /// </summary>
        /// <param name="task"></param>
        /// <param name="type"></param>
        /// <param name="finishDevice"></param>
        void NotifyTaskExcuteStatus(DeviceBaseAbstract finishDevice, TTask task, TaskExcuteStepStatusEnum type);
    }
}
