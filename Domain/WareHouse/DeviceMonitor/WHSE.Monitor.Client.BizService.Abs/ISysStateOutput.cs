using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WHSE.Monitor.Framework.Model.DataModel;

namespace WHSE.Monitor.Client.BizService.Abs
{
	public delegate bool SysStateOutputDelegate(string sysName, SysStateEnum state);

	public interface ISysStateOutput
	{
		event SysStateOutputDelegate SysStateOutputEvent;

		DateTime GetLatestHeartbeatTime(string sysName);
	}
}
