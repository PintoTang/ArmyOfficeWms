using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.StringCharTaskHandle
{
  public  interface IStringCharTaskNotifyCentre
    {
        /// <summary>
        /// 指令状态监听者列表
        /// </summary>
      Pool<IHandleStringCharTaskStatus> TaskStatusListenerList { get; }
        /// <summary>
        /// 注册指令监听者
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
      OperateResult RegisterTaskStatusListener(IHandleStringCharTaskStatus listener);
        /// <summary>
        /// 解注册指令监听者
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
      OperateResult UnRegisterTaskStatusListener(IHandleStringCharTaskStatus listener);
       /// <summary>
       /// 通知任务完成
       /// </summary>
      /// <param name="device"></param>
       /// <param name="task"></param>
      void NotifyStringCharTaskFinish(DeviceBaseAbstract device, StringCharTask task);
    }
}
