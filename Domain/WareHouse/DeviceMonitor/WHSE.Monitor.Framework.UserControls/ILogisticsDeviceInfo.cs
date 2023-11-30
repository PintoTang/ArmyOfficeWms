using CL.Framework.CmdDataModelPckg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace WHSE.Monitor.Client.UserControls
{
	public interface ILogisticsDeviceInfo
	{
		/// <summary>
		/// 物流运动线路
		/// </summary>
		DoubleAnimationBase getPath(DeviceName from, DeviceName to);
	}

}
