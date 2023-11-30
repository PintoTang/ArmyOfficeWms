using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Threading;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Load.ViewModel
{
    public class LoadPlaceViewModel : DeviceViewModelAbstract<LoadPlace>
	{
        public LoadPlaceViewModel(LoadPlace device) : base(device)
        {
        }

        private void ClearPackageMsg()
		{
			lock (PackageLst)
			{
				if (PackageLst.Count >= 100)
				{
					PackageLst.RemoveAt(0);
				}
			}
		}

		public void ShowPackage(Package package)
		{
			if (System.Windows.Application.Current.Dispatcher != null)
				System.Windows.Application.Current.Dispatcher.Invoke(new Action<Package>((p) =>
				{
					CurrentPackage = p;
					ClearPackageMsg();
					PackageLst.Add(p);
				}),DispatcherPriority.Background,package);
		}


		private ObservableCollection<Package> packageLst = new ObservableCollection<Package>();

		public ObservableCollection<Package> PackageLst
		{
			get
			{
				return packageLst;
			}
			set
			{
				packageLst = value;
				RaisePropertyChanged("PackageLst");
			}
		}

		private Package currentPackage;
		public Package CurrentPackage
		{
			get
			{
				return currentPackage;
			}
			set
			{
				if (!currentPackage.PackageId.Equals(value.PackageId))
				{
					currentPackage = value;
					RaisePropertyChanged("CurrentPackage");
				}
			}
		}

	
		//设备连接 参数 
		private bool _isCheckedConnStatus = false;
		private bool _isEnabledConnStatus = true;
		private SolidColorBrush _gbConnStatus;
		private string _lblConnContent = "离线";
		/// <summary>
		/// 连接状态  IsChecked
		/// </summary>
		[Custom(Alias = "ConnectState")]
		public bool IsCheckedConnStatus
		{
			get
			{
				return _isCheckedConnStatus;
			}
			set
			{
				if (_isCheckedConnStatus != value)
				{
					_isCheckedConnStatus = value;
					RaisePropertyChanged("IsCheckedConnStatus");
				}
			}
		}

     

		/// <summary>
		/// 连接状态  IsEnabled
		/// </summary>
		public bool IsEnabledConnStatus
		{
			get
			{
				return _isEnabledConnStatus;
			}
			set
			{
				if (_isEnabledConnStatus != value)
				{
					_isEnabledConnStatus = value;
					RaisePropertyChanged("IsEnabledConnStatus");
				}
			}
		}


		private bool _isCheckedTaskStatus = false;
		private bool _isEnabledTaskStatus = true;
		private SolidColorBrush _gbTaskStatus;
		private string _lblTaskContent = "无";
		/// <summary>
		/// 连接状态  IsChecked
		/// </summary>
		public bool IsCheckedTaskStatus
		{
			get
			{
				return _isCheckedTaskStatus;
			}
			set
			{
				if (_isCheckedTaskStatus != value)
				{
					_isCheckedTaskStatus = value;
					RaisePropertyChanged("IsCheckedTaskStatus");
				}
			}
		}
		/// <summary>
		/// 连接状态  IsEnabled
		/// </summary>
		public bool IsEnabledTaskStatus
		{
			get
			{
				return _isEnabledTaskStatus;
			}
			set
			{
				if (_isEnabledTaskStatus != value)
				{
					_isEnabledTaskStatus = value;
					RaisePropertyChanged("IsEnabledTaskStatus");
				}
			}
		}
       
       

		public override void NotifyAttributeChange(string attributeName, object newValue)
		{
			if (attributeName.Equals("ConnectState"))
			{
				RaisePropertyChanged("IblConnContent");
			}
			if (attributeName.Equals("CurrentPackage"))
			{
				Package package = (Package)newValue;
				ShowPackage(package);
			}
			if (attributeName.Equals("UnFinishedTask"))
			{
				RaisePropertyChanged("UnfinishedOrderList");
			}
            if (attributeName.Equals("WorkState"))
		    {
                RaisePropertyChanged("PackIconTaskName");
		    }
		}
	}
}
