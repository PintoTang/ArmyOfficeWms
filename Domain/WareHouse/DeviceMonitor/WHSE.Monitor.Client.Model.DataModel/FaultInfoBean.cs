using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.Model.DataModel
{
	public class FaultInfoBean
	{
		/// <summary>
		/// 故障代码
		/// </summary>
		public string FalutCode { set; get; }

		/// <summary>
		/// 故障等级
		/// </summary>
		public FaultLevelEnum FaultLevel { set; get; }

		/// <summary>
		/// 故障信息
		/// </summary>
		public string FaultMessage { set; get; }


		public DateTime FaultTime { get; set; }

		public string AreaName { get; set; }

		public string DeviceName { get; set; }
	}
}
