using System;
using System.Threading;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.OrderGenerate
{
    public class OrderGenerate : IOrderGenerate
    {
        private readonly OrderDataAbstract OrderDatabaseHandler;
        public OrderGenerate()
        {
            OrderDatabaseHandler = DependencyHelper.GetService<OrderDataAbstract>();
            maxOrderID = OrderDatabaseHandler.GetMaxIDExist("ORDERID", "T_BO_ORDEROPERATE", "");//该方法如果不存在数据返回0
        }
        private object objLock = new object();
        /// <summary>
        /// 整个应用程序ORDERID唯一，且自增。目前暂时从本地数据库重读取获得初始值
        /// </summary>
        private int maxOrderID = 1;

        public OperateResult<Order> GenerateOrder(Order destOrder)
        {
            lock (objLock)
            {
                destOrder.OrderId = Interlocked.Increment(ref maxOrderID);
                destOrder.CreateTime = DateTime.Now;

                if (destOrder.OrderId == -0x7FFFFFFF)
                {
                    return OperateResult.CreateFailedResult<Order>(null, "指令生成失败，无效的订单号");
                }
                string msg = string.Format("指令生成成功，指令：{0}", destOrder);
                OperateResult<Order> result = OperateResult.CreateSuccessResult(destOrder);
                result.Message = msg;
                return result;
            }
        }


        public int GetGlobalNewTaskId()
        {
            lock (objLock)
            {
                return Interlocked.Increment(ref maxOrderID);
                //return maxOrderID;
            }
        }
    }
}
