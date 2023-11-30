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
    /// <summary>
    /// 能进行指令接收
    /// </summary>
    public interface IOrderReceive
    {
        /// <summary>
        /// 接收者的编号
        /// </summary>
        int ReceiverId { get; set; }

        /// <summary>
        /// 接收入库指令前缀
        /// </summary>
        List<string> InSrcAddrPrefixList { get; set; }
        /// <summary>
        /// 接收出库指令前缀
        /// </summary>
        List<string> OutSrcAddrPrefixList { get; set; }
        /// <summary>
        /// 接收移库指令前缀
        /// </summary>
        List<string> MoveSrcAddrPrefixList { get; set; }

        DeviceName OrderReceiverName { get; }

        /// <summary>
        /// 接受订单
        /// </summary>
        /// <param name="order">订单ExOrder</param>
        OperateResult ReceiveOrder(ExOrder order);

        /// <summary>
        /// 是否可以接收指令
        /// </summary>
        /// <returns></returns>
        OperateResult IsCanRecieveOrder(ExOrder order);


        event OrderUpdateStatusDelegate OrderUpdateStatusEvent;

    }

    public delegate OperateResult OrderUpdateStatusDelegate(DeviceName deviceName, ExOrder order, TaskHandleResultEnum type);
}
