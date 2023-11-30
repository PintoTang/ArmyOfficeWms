using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CL.Framework.CmdDataModelPckg
{
	/// <summary>
	/// 订单类型枚举
	/// </summary>
	public enum OrderTypeEnum
	{
		/// <summary>
		/// 入库
		/// </summary>
		[Description("入库指令")]
		In,

		/// <summary>
		/// 出库
		/// </summary>
		[Description("出库指令")]
		Out,

		/// <summary>
		/// 移库(仓位到仓位，站台到站台等)
		/// </summary>
		[Description("移库指令")]
		Move,
		/// <summary>
		/// 拣选出库
		/// </summary>
		[Description("拣选指令")]
		PickOut,
		/// <summary>
		/// 盘点出库
		/// </summary>
		[Description("盘点指令")]
		Inventory,
		/// <summary>
		/// 未知类型
		/// </summary>
		[Description("未知类型")]
		UnKnow,

	}
}
