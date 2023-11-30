using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Interface
{
	/// <summary>
	/// 指令创建接口
	/// </summary>
	public interface IOrderGenerate
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
		OperateResult<Order> GenerateOrder(Order  destOrder);

        /// <summary>
        /// 获取唯一ID
        /// </summary>
        /// <returns></returns>
        int GetGlobalNewTaskId();

	}
}
