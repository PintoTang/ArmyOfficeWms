using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WHSE.Monitor.Framework.Model.DataModel;
using WHSE.Monitor.Client.Model.DataPool;

namespace WHSE.Monitor.Framework.UserControls
{
	public class AreaEntryViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public AreaEntryViewModel()
		{

			PackageMoveDataPool packageMoveDataPool = PackageMoveDataPool.Instance;
			packageMoveDataPool.RegisterViewRefreshDelegate(PackageMove);

			FaultInfo.Instance.ChangeAreaStateEnvent += Instance_ChangeAreaStateEnvent;


		}



		public void Instance_ChangeAreaStateEnvent()
		{

			RefreshAreaState();
			Console.WriteLine(AreaName + "" + AreaEntryState);
		}

		private void RefreshAreaState()
		{
			IEnumerable<FloorBean> floors = FloorControlList.Instance.FloorBeanList.Where(x => x.AreaName == AreaName);
			if (floors == null)
			{
				throw new Exception("当前区域没有楼层对象");
			}
			bool result = false;
			DeviceDisplayStateEnum state = DeviceDisplayStateEnum.Normal;
			Application.Current.Dispatcher.Invoke(new Action(() =>
			{
				foreach (var floor in floors)
				{
					if (result)
					{
						break;
					}
					ResetDisplyState(ref result, ref state, floor);
				}
			}));
			AreaEntryState = state;

		}

		private void ResetDisplyState(ref bool result, ref DeviceDisplayStateEnum state, FloorBean floor)
		{
			foreach (var device in floor.Children)
			{
				if (result)
				{
					break;
				}

				if (device.DataContext != null)
				{
					DeviceBase de = (device.DataContext as DeviceBase);
					if (de.DeviceRunningMode == DeviceManualAutomaticEnum.Manual)
					{

						state = DeviceDisplayStateEnum.Manual;
					}
					if (de.EmergencyStop == true)
					{
						AreaEntryState = DeviceDisplayStateEnum.EmergencyStop;
					}
					if (de.DeviceState == DeviceStateEnum.Failure)
					{
						result = true;
						state = DeviceDisplayStateEnum.Failure;
						break;
					}
				}


			}
		}

		private List<PackageMoveInfoBean> _packageMoveInfoList;
		public List<PackageMoveInfoBean> PackageMoveInfoList
		{
			get
			{
				return _packageMoveInfoList;
			}
			set
			{
				_packageMoveInfoList = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("PackageMoveInfoList"));
				}

			}
		}
		private List<object> _deviceList = new List<object>();


		public List<object> DeviceList
		{
			get { return _deviceList; }
			set { _deviceList = value; }
		}


		private DeviceDisplayStateEnum _areaState;
		/// <summary>
		/// 当前区域的状态
		/// </summary>
		public DeviceDisplayStateEnum AreaEntryState
		{
			get { return _areaState; }
			set
			{
				_areaState = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("AreaEntryState"));
				}

			}
		}
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

		private string areaName;

		public string AreaName
		{
			get { return areaName; }
			set
			{
				areaName = value;

			}
		}



		public bool PackageMove(List<PackageMoveInfoBean> packageMoveInfoBeanList)
		{
			if (packageMoveInfoBeanList == null)
			{
				return false;
			}
			PackageMoveInfoList = packageMoveInfoBeanList;
			foreach (var packMoveInfoItem in PackageMoveInfoList)
			{
				//以后动画代码写在当前位置
				Application.Current.Dispatcher.Invoke(new Action(() =>
				{
					string barcode = packMoveInfoItem.PackageBarcode;
					string deviceName = packMoveInfoItem.CurrDevice.FullName;
					DeviceBase device = FloorControlList.Instance.FindDevice(deviceName);
					UpdateDeviceStyle(packMoveInfoItem, barcode, device);
					UpdatePackageMoveInfoList(packMoveInfoItem, barcode, device);
				}));

			}
			return true;
		}

		private void UpdatePackageMoveInfoList(PackageMoveInfoBean packMoveInfoItem, string barcode, DeviceBase device)
		{
			var query = PackageList.Instance.PackageBeanList.Where(x => x.PackageBarcode == barcode);
			if (query.Count() > 0)
			{
				PackageBean packageBean = PackageList.Instance.PackageBeanList.Where(x => x.PackageBarcode == barcode).FirstOrDefault();

		
				//从前一个设备中移除包
				packageBean.CurrertLocaton.IsHasTask = false;
				var frontDevicePackage = packageBean.CurrertLocaton.PackageMoveInfoList.Where(x => x.PackageBarcode == barcode).FirstOrDefault();
				packageBean.CurrertLocaton.PackageMoveInfoList.Remove(frontDevicePackage);
				
				//将新的设备赋给包的位置信息
				packageBean.CurrertLocaton = device;
				packageBean.PackageMoveInfo = packMoveInfoItem;

				if (device.IsEndDevice)
				{
					PackageList.Instance.PackageBeanList.Remove(packageBean);
				}
			}
			else
			{
				if (!device.IsEndDevice)
				{
					PackageBean packageBean = new PackageBean();
					packageBean.PackageBarcode = barcode;
					packageBean.CurrertLocaton = device;
					packageBean.PackageMoveInfo = packMoveInfoItem;

					PackageList.Instance.PackageBeanList.Add(packageBean);
				}

			}
		}

		private void UpdateDeviceStyle(PackageMoveInfoBean packMoveInfoItem, string barcode, DeviceBase device)
		{
			if (device != null)
			{
				device.IsHasTask = true;
				if (device.PackageMoveInfoList.Where(x => x.PackageBarcode == barcode).Count() > 0)
				{
					var packageMoveInfo = PackageMoveInfoList.Where(x => x.PackageBarcode == barcode).FirstOrDefault();
					if (!device.IsEndDevice)
					{
						packageMoveInfo = packMoveInfoItem;
					}
					else
					{
						device.PackageMoveInfoList.Remove(packageMoveInfo);
					}

				}
				else
				{
					if (!device.IsEndDevice)
					{
						device.PackageMoveInfoList.Add(packMoveInfoItem);
					}

				}

			}
		}



	}
}
