using System;
using System.Collections.Generic;
using System.Text;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Interface
{
    /// <summary>
    /// 指令分发器的接口
    /// </summary>
    public interface IOrderAllocator
    {
        /// <summary>
        /// 指令分发器的名字
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 分发指令
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        OperateResult<IOrderReceive> Alloc(ExOrder order);


        /// <summary>
        /// 注册指令接收者
        /// </summary>
        /// <param name="orderReceiver"></param>
        void RegisterOrderReceiver(IOrderReceive orderReceiver);

        /// <summary>
        /// 解注册指令接收者
        /// </summary>
        /// <param name="orderReceiver"></param>
        void UnregisterOrderReceiver(IOrderReceive orderReceiver);

        OperateResult UpdateOrderStatus(DeviceName deviceName, ExOrder order, TaskHandleResultEnum type);

        event OrderUpdateStatusDelegate UpdateOrderStatusEvent;

    }


}
