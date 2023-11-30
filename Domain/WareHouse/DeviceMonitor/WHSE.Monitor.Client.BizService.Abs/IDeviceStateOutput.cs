using CL.Framework.CmdDataModelPckg;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WHSE.Monitor.Framework.Model.DataModel;

namespace WHSE.Monitor.Client.BizService.Abs
{
	public delegate bool DeviceFaultStateOutputDelegate(DeviceName deviceName, List<byte> stateList);
	public delegate bool DeviceManualAutomaticOutputDelegate(DeviceName deviceName, DeviceManualAutomaticEnum manualAutomaticEnum);

	public interface IDeviceStateOutput
	{
		event DeviceFaultStateOutputDelegate DeviceFaultStateOutputEvent;
		event DeviceManualAutomaticOutputDelegate DeviceManualAutomaticOutputEvent;
	}
}
