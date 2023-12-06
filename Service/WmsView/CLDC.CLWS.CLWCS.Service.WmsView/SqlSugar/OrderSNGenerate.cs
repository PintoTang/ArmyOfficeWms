using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using Infrastructrue.Ioc.DependencyFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.WmsView.SqlSugar
{
    public class OrderSNGenerate: IOrderSNGenerate
    {
        private readonly WmsDataAbstract OrderDatabaseHandler;
        public OrderSNGenerate()
        {
            OrderDatabaseHandler = DependencyHelper.GetService<WmsDataAbstract>();
            maxOrderID = OrderDatabaseHandler.GetMaxID("Id", "t_InOrder", "");//该方法如果不存在数据返回0
        }
        private object objLock = new object();
        /// <summary>
        /// 整个应用程序ORDERID唯一，且自增。目前暂时从本地数据库重读取获得初始值
        /// </summary>
        private long maxOrderID = 1;

        public OperateResult<InOrder> GenerateOrder(InOrder destOrder)
        {
            lock (objLock)
            {
                destOrder.Id = Interlocked.Increment(ref maxOrderID);
                destOrder.OrderSN = "In_"+ Interlocked.Increment(ref maxOrderID);
                destOrder.CreatedTime = DateTime.Now;

                if (destOrder.Id == -0x7FFFFFFF)
                {
                    return OperateResult.CreateFailedResult<InOrder>(null, "指令生成失败，无效的订单号");
                }
                string msg = string.Format("指令生成成功，指令：{0}", destOrder);
                OperateResult<InOrder> result = OperateResult.CreateSuccessResult(destOrder);
                result.Message = msg;
                return result;
            }
        }


        public long GetGlobalNewTaskId()
        {
            lock (objLock)
            {
                return Interlocked.Increment(ref maxOrderID);
            }
        }
    }
}
