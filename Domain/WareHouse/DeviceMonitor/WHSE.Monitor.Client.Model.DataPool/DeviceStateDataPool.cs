using CL.Framework.CmdDataModelPckg;
using CLDC.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHSE.Monitor.Client.BizService.Abs;
using WHSE.Monitor.Framework.Model.DataModel;

namespace WHSE.Monitor.Client.Model.DataPool
{
	public class DeviceStateDataPool : BaseDataPool<DeviceName, DeviceStateAndDelegateInfo>
	{
		#region 折叠单例模式的实现代码
		private static DeviceStateDataPool _instance;
		private DeviceStateDataPool()
		{
		}

		public static DeviceStateDataPool Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeviceStateDataPool();
				}
				return _instance;
			}
		}

		public static void TI_ClearInstance()
		{
			_instance = null;
		}
		#endregion

		IDeviceStateOutput _deviceStateOutputAbs = null;

        public void SetDeviceStateOutputAbs(IDeviceStateOutput deviceStateOutputAbs)
		{
			this._deviceStateOutputAbs = deviceStateOutputAbs;
			this._deviceStateOutputAbs.DeviceFaultStateOutputEvent += this.DeviceFaultStateInput;
			this._deviceStateOutputAbs.DeviceManualAutomaticOutputEvent += this.DeviceManualAutomaticInput;
		}

		private bool DeviceFaultStateInput(DeviceName deviceName, List<byte> faultStateList)
		{
			CreateNewValueIfNotInDict(deviceName);
			_dataPoolDict[deviceName].FaultStateList = faultStateList;
			return true;
		}

		private bool DeviceManualAutomaticInput(DeviceName deviceName, DeviceManualAutomaticEnum manualAutomaticEnum)
		{
			CreateNewValueIfNotInDict(deviceName);
			_dataPoolDict[deviceName].DeviceManualAutomaticEnum = manualAutomaticEnum;
			return true;
		}

		public void RegisterViewRefreshDelegate(DeviceName deviceName, DeviceStateViewRefreshDelegate deviceStateViewRefreshDelegate)
		{
			if (null == deviceName)
			{
				string msg = "界面层尝试注册一个空deviceName的回调委托，说明界面层有严重BUG。需要界面层将该方法的调用迁移到已经能获取到deviceName的位置调用。";
				Log.getDebugFile().Debug(msg);
				throw new Exception(msg);
			}

			CreateNewValueIfNotInDict(deviceName);
			_dataPoolDict[deviceName].DeviceStateViewRefreshDelegate = deviceStateViewRefreshDelegate;
		}

		public DeviceStateBean GetDeviceState(DeviceName deviceName)
		{
			return GetDataPoolRecord(deviceName);
		}
	}
}
