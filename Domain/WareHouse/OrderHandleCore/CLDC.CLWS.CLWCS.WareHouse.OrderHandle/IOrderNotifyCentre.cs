using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.OrderHandle
{
    /// <summary>
    /// 指令状态通知中心
    /// </summary>
    public interface IOrderNotifyCentre
    {
        /// <summary>
        /// 指令状态监听者列表
        /// </summary>
        Pool<IHandleOrderExcuteStatus> OrderStatusListenerList { get; }
        /// <summary>
        /// 注册指令监听者
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        OperateResult RegisterOrderStatusListener(IHandleOrderExcuteStatus listener);
        /// <summary>
        /// 解注册指令监听者
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        OperateResult UnRegisterOrderStatusListener(IHandleOrderExcuteStatus listener);
        /// <summary>
        /// 通知指令变化
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="order"></param>
        /// <param name="type"></param>
        void NotifyOrderChange(DeviceName deviceName, ExOrder order, TaskHandleResultEnum type);
    }
}
