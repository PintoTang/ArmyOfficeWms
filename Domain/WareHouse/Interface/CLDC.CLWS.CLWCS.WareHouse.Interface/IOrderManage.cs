using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Interface
{
    /// <summary>
    /// 指令管理接口
    /// </summary>
    public interface IOrderManage
    {
        DataObservablePool<ExOrder> GetAllUnAllocatedOrder();

        DataObservablePool<ExOrder> GetAllUnFinishedOrderByOwnerId(int ownerId);

        OperateResult UpdateOrder(ExOrder order);

        void UpdateOrderAsync(ExOrder order);

        OperateResult<ExOrder> GetMemoryExOrderByOrderId(int orderId);

        OperateResult<ExOrder> GetDataBaseExOrderByOrderId(int orderId);


        OperateResult<Order> GenerateOrder(Order destOrder);

        OperateResult HandleRestoreData();

        /// <summary>
        /// 移除内存
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        OperateResult RemoveOrder(ExOrder order);

    }
}
