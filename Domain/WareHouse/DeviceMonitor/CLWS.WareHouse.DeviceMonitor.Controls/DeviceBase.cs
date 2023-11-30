using CL.Framework.CmdDataModelPckg;
using Framework.LanguageConverter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WHSE.Monitor.Framework.Model.DataModel;
using WHSE.Monitor.Client.Model.DataPool;

namespace WHSE.Monitor.Framework.UserControls
{
	/// <summary>
	/// 设备基类
	/// </summary>
	public abstract class DeviceBase : LogisticsDeviceControlBase, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public DeviceName CurrentDevice;

		#region 方法-刷新设备状态       
		public void RefreshDeviceState(DeviceStateBean state)
		{
			if (state != null)
			{

				DeviceRunningMode = state.DeviceManualAutomaticEnum;

				FaultStateList = state.FaultStateList;
				
				if (state.FaultStateList != null)
				{
					int count = state.FaultStateList.Where(x => x > 0).Count();
					if (count == 0)
					{
						DeviceState = DeviceStateEnum.Normal;
					}
					else
					{
						DeviceState = DeviceStateEnum.Failure;
					}
				}
	
				ConvertDisplyState();
				FaultInfo.Instance.ChangeDeviceFaultState(DeviceName, state);

			}

		}

		#endregion

		#region 根据设备状态转化前台样式
		void ConvertDisplyState()
		{

			if (DeviceState == DeviceStateEnum.Failure)
			{
				DeviceDisplayState = DeviceDisplayStateEnum.Failure;
				return;
			}
			if (EmergencyStop == true)
			{
				DeviceDisplayState = DeviceDisplayStateEnum.EmergencyStop;
				return;
			}
			if (DeviceRunningMode == DeviceManualAutomaticEnum.Manual)
			{
				DeviceDisplayState = DeviceDisplayStateEnum.Manual;
				return;
			}
			if (IsHasTask)
			{
				DeviceDisplayState = DeviceDisplayStateEnum.Running;
				return;
			}
			DeviceDisplayState = DeviceDisplayStateEnum.Normal;

		}
		#endregion

		#region 属性-设备名
		private string _deviceName;
		/// <summary>
		/// 设备名称
		/// </summary>
		public string DeviceName
		{
			get
			{
				if (CurrentDevice != null)
				{
					//string name = CNNameConverter.From(CurrentDevice);
					return _deviceName;
				}
				else
				{
					return _deviceName;
				}

			}
			set
			{
				_deviceName = value;
				if (!string.IsNullOrEmpty(value))
				{
					CurrentDevice = new DeviceName(DeviceName);
					DeviceStateDataPool.Instance.RegisterViewRefreshDelegate(CurrentDevice, RefreshDeviceState);
				}

			}
		}
		#endregion

		#region 属性-设备中文名

		/// <summary>
		/// 设备中文名
		/// </summary>
		public string DeviceCNName
		{
			get
			{
				return CNNameConverter.From(CurrentDevice);
			}
		}
		#endregion

		#region 属性-运行模式
		private DeviceManualAutomaticEnum _deviceRunningModeEnum;
		/// <summary>
		/// 设备运行模式
		/// </summary>
		public DeviceManualAutomaticEnum DeviceRunningMode
		{
			get { return _deviceRunningModeEnum; }
			set
			{
				_deviceRunningModeEnum = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("DeviceRunningMode"));
				}

			}
		}
		#endregion

		#region 属性-设备状态
		private DeviceStateEnum _deviceState;
		/// <summary>
		/// 设备运行状态
		/// </summary>
		public DeviceStateEnum DeviceState
		{
			get { return _deviceState; }
			set
			{
				_deviceState = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("DeviceState"));
				}
			}
		}
		#endregion

		#region 属性-急停状态

		private bool _isEmergencyStop;
		/// <summary>
		/// 急停状态
		/// </summary>
		public bool EmergencyStop
		{
			get { return _isEmergencyStop; }
			set
			{
				_isEmergencyStop = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("EmergencyStop"));
				}
			}
		}

		#endregion

		#region 属性-故障信息


		private FaultInfoBean _faultInfo;
		/// <summary>
		/// 故障信息
		/// </summary>
		public FaultInfoBean FaultInfomation
		{
			get
			{
				return _faultInfo;
			}
			set
			{
				_faultInfo = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("FaultInfomation"));
				}
			}
		}
		#endregion

		#region 属性-是否终点设备


		private bool _isEndDevice;
		/// <summary>
		/// 是否为终点设备
		/// </summary>
		public bool IsEndDevice
		{
			get { return _isEndDevice; }
			set { _isEndDevice = value; }
		}
		#endregion

		#region 属性-设备搬运货品集合

		private ObservableCollection<PackageMoveInfoBean> _packagesMoveInfoList = new ObservableCollection<PackageMoveInfoBean>();

		public ObservableCollection<PackageMoveInfoBean> PackageMoveInfoList
		{
			get
			{
				return _packagesMoveInfoList;
			}
			set
			{
				_packagesMoveInfoList = value;
				//if (PropertyChanged != null)
				//{
				//	PropertyChanged(this, new PropertyChangedEventArgs("PackageMoveInfoList"));
				//}
			}
		}
		#endregion

		#region 属性-故障信息字节列表

		private List<byte> _faultStateList;
		/// <summary>
		/// 详细故障信息
		/// </summary>
		public List<byte> FaultStateList
		{
			get
			{
				return _faultStateList;
			}
			set
			{
				_faultStateList = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("FaultStateList"));
				}
			}
		}
		#endregion

		#region 属性-设备显示状态
		private DeviceDisplayStateEnum _deviceDisplayState;
		public DeviceDisplayStateEnum DeviceDisplayState
		{
			get
			{
				return _deviceDisplayState;
			}
			set
			{
				_deviceDisplayState = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("DeviceDisplayState"));
				}

			}
		}
		#endregion

		#region 属性-是否有任务

		private bool _isHasTask;
		/// <summary>
		/// 是否有执行任务
		/// true:有
		/// false:无
		/// </summary>
		public bool IsHasTask
		{
			get
			{
				return _isHasTask;
			}
			set
			{
				if (!IsEndDevice)
				{
					_isHasTask = value;

					if (PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangedEventArgs("IsHasTask"));
					}
					ConvertDisplyState();
				}
				
			}
		}
		#endregion


	}
}
