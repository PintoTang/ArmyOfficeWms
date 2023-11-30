using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WHSE.Monitor.Client.UserControls
{
	public interface ILogisticsDeviceControl
	{
		/// <summary>
		/// 运动速度
		/// </summary>
		 double Speed { get; set; }
		/// <summary>
		/// 运动起点
		/// </summary>
		 Point BeginPonit { get; }
		/// <summary>
		/// 运动重点
		/// </summary>
		 Point EndPonit { get; }
	}
}
