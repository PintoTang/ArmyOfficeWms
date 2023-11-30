using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.Model.DataModel
{
	public enum FaultLevelEnum
	{
		/// <summary>
		/// 提示，系统自动处理 
		/// </summary>
		Level_1,
		/// <summary>
		/// 单机故障
		/// </summary>
		Level_2,
		/// <summary>
		/// 单线故障
		/// </summary>
		Level_3,
		/// <summary>
		/// 关键动线故障，WCS宕机
		/// </summary>
		Level_4,
		/// <summary>
		/// 关键区域故障，WMS宕机
		/// </summary>
		Level_5,
	}
}
