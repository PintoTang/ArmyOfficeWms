using CLDC.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHSE.Monitor.Framework.Model.DataModel;


namespace WHSE.Monitor.Client.Model.DataPool
{
	public delegate void DeviceStateViewRefreshDelegate(DeviceStateBean state);

	public class DeviceStateAndDelegateInfo : DeviceStateBean
	{
		private DeviceStateViewRefreshDelegate _deviceStateViewRefreshDelegate = null;
		public DeviceStateViewRefreshDelegate DeviceStateViewRefreshDelegate
		{
			set
			{
				lock (this)
				{
					if (value != _deviceStateViewRefreshDelegate)
					{
						_deviceStateViewRefreshDelegate = value;
					}
				}
			}
		}

		public new List<byte> FaultStateList 
		{
			set
			{
				if (null == value)
				{
					Log.getDebugFile().Debug("设备故障List为null。Monitor程序有严重bug！");
					return;
				}

				if (0 == value.Count)
				{
					Log.getDebugFile().Debug("下位系统上报的设备故障List长度为0。需要下位系统定位BUG！");
					return;
				}

				lock (this)
				{
					if (null == _faultStateList)
					{
						UpdateAndTryCallDelegate(value);
					}
					else
					{
						if(!IsContentEqual(value , _faultStateList))
						{
							UpdateAndTryCallDelegate(value);
							return;
						}		
					}
				}
			}
		}

		private bool IsContentEqual(List<byte> value, List<byte> faultStateList)
		{
			if (value == faultStateList)
			{
				return true;
			}

			int valueListCount = value.Count;
			if (valueListCount != faultStateList.Count)
			{
				Log.getDebugFile().Debug("下位系统上报的两次设备故障List长度不一样。需要下位系统定位BUG！");
				return false;
			}

			for (int i = 0; i < valueListCount; i++)
			{
				if (value[i] != faultStateList[i])
				{
					return false;
				}
			}
			return true;
		}

		private void UpdateAndTryCallDelegate(List<byte> newFaultStateList)
		{
			_lastRxFaultTime = DateTime.Now;
			_faultStateList = newFaultStateList;
			if (null != _deviceStateViewRefreshDelegate)
			{
				_deviceStateViewRefreshDelegate(this);
			}
		}

		public new DeviceManualAutomaticEnum DeviceManualAutomaticEnum
		{
			set
			{
				lock (this)
				{
					if (value != _deviceManualAutomaticEnum)
					{
						_deviceManualAutomaticEnum = value;
						if (null != _deviceStateViewRefreshDelegate)
						{
							_deviceStateViewRefreshDelegate(this);
						}
					}
				}
			}
		}
	}
}
