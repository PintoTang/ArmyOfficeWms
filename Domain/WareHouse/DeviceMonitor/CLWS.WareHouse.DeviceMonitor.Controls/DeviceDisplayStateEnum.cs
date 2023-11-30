using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.UserControls
{
	public enum DeviceDisplayStateEnum
	{
		/// <summary>
		/// 正常状态
		/// </summary>
		Normal,
		/// <summary>
		/// 故障
		/// </summary>
		Failure,
		/// <summary>
		/// 急停
		/// </summary>
		EmergencyStop,
		/// <summary>
		/// 手动
		/// </summary>
		Manual,
		/// <summary>
		/// 工作中
		/// </summary>
		Running
	}
}
