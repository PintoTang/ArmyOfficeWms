using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.UserControls
{
	/// <summary>
	/// 区域状态
	/// </summary>
	public enum AreaState
	{
		/// <summary>
		/// 正常
		/// </summary>
		Auto=0,
		/// <summary>
		/// 故障
		/// </summary>
		Failure=1,
		/// <summary>
		/// 手动
		/// </summary>
		Manual=2

	}
}
