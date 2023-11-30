using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CL.Framework.CmdDataModelPckg
{
	/// <summary>
	/// 设备使用状态(Normal正常、Fault故障维修中、Remove被移出货架)
	/// </summary>
	public enum DeviceStatusEnum
	{
		/// <summary>
		/// 
		/// </summary>
		[Description("正常")]
		Normal = 0,
		/// <summary>
		/// 
		/// </summary>
		[Description("故障维修中")]
		Fault = 1,
		/// <summary>
		/// 
		/// </summary>
		[Description("被移出货架")]
		Remove = 2,
	}
}
