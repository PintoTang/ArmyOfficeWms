using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;

namespace CLDC.CLWS.CLWCS.Service.WmsView.SqlSugar
{
    public interface IOrderSNGenerate
    {

        /// <summary>
        /// 创建指令
        /// </summary>
        /// <param name="orderType"></param>
        /// <param name="startAddr"></param>
        /// <param name="destAddr"></param>
        /// <param name="palletBarcode"></param>
        /// <param name="priority"></param>
        /// <param name="documentCode"></param>
        /// <param name="backFlag"></param>
        /// <returns></returns>
        OperateResult<Order> GenerateOrder(Order destOrder);

        /// <summary>
        /// 获取唯一ID
        /// </summary>
        /// <returns></returns>
        long GetGlobalNewTaskId();
    }
}
