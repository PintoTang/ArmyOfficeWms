using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Threading;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Station.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Station.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Station.ViewModel
{
    public class StationDeviceViewModel : DeviceViewModelAbstract<StationDeviceAbstract>
    {
        public StationDeviceViewModel(StationDeviceAbstract device)
            : base(device)
        {
            OpcElementViewModel = new OpcElementViewModel();
            _stationControl = Device.DeviceControl;
            InitOpcElementViewModel();
            InitTransportMessageViewModel();
        }


        public TransportMessageViewModel TransportMessageViewModel { get; private set; }


        private void InitTransportMessageViewModel()
        {
            TransportMessageViewModel = new TransportMessageViewModel();
            TransportMessageViewModel.TransportViewHeight = Device.WorkSize * 45.0;
            TransportMessageViewModel.UnfinishedTransportList = Device.UnFinishedTask.DataPool;

        }

        private readonly StationDeviceControlAbstract _stationControl;
        public OpcElementViewModel OpcElementViewModel { get; private set; }

        private void InitOpcElementViewModel()
        {
            OpcElementViewModel.OpcElementViewHeight = DeviceOpcElement.Datablocks.Count * 50.0;
            OpcElementViewModel.DeviceOpcElement = DeviceOpcElement;
            OpcElementViewModel.RefreshAllData = _stationControl.Communicate.RefreshAllData;
            OpcElementViewModel.Write = _stationControl.Communicate.Write;
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
                }), DispatcherPriority.Background, package);
        }


        public OpcElement DeviceOpcElement
        {
            get
            {
                if (Device.DeviceControl == null)
                {
                    return new OpcElement();
                }
                return Device.DeviceControl.Communicate.OpcElement;
            }
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


        //DisPlayDevice
        private string _DisplayTitle = "标题";
        /// <summary>
        /// 显示的标题
        /// </summary>
        public string DisplayTitle
        {
            get
            {
                return _DisplayTitle;
            }
            set
            {
                if (_DisplayTitle != value)
                {
                    _DisplayTitle = value;
                    RaisePropertyChanged("DisplayTitle");
                }
            }
        }
        private string _DisplayContent = "默认内容";
        /// <summary>
        /// 显示的内容
        /// </summary>
        public string DisplayContent
        {
            get
            {
                return _DisplayContent;
            }
            set
            {
                if (_DisplayContent != value)
                {
                    _DisplayContent = value;
                    RaisePropertyChanged("DisplayContent");
                }
            }
        }


        //IdentityDevice

        private string _IdentityCurBarCode = "0";
        /// <summary>
        /// 扫描的条码
        /// </summary>
        public string IdentityCurBarCode
        {
            get
            {
                return _IdentityCurBarCode;
            }
            set
            {
                if (_IdentityCurBarCode != value)
                {
                    _IdentityCurBarCode = value;
                    RaisePropertyChanged("IdentityCurBarCode");
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
        public SolidColorBrush GBConnStatus
        {
            get
            {
                if (_gbConnStatus == null)
                {
                    _gbConnStatus = GetColorBrushByStatu(0);
                }
                return _gbConnStatus;
            }
            set
            {
                if (_gbConnStatus != value)
                {
                    _gbConnStatus = value;
                    RaisePropertyChanged("GBConnStatus");
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
        public SolidColorBrush GBTaskStatus
        {
            get
            {
                if (_gbTaskStatus == null)
                {
                    _gbTaskStatus = GetColorBrushByStatu(0);
                }
                return _gbTaskStatus;
            }
            set
            {
                if (_gbTaskStatus != value)
                {
                    _gbTaskStatus = value;
                    RaisePropertyChanged("GBTaskStatus");
                }
            }
        }



        /// <summary>
        /// 任务连接
        /// </summary>
        public string IblTaskContent
        {
            get
            {
                return _lblTaskContent;

            }
            set
            {
                if (_lblTaskContent != value)
                {
                    _lblTaskContent = value;
                    if (_lblTaskContent == "有")
                    {
                        IsCheckedTaskStatus = true;
                        IsEnabledTaskStatus = true;
                        GBTaskStatus = GetColorBrushByStatu(1);
                    }
                    else
                    {
                        IsCheckedTaskStatus = false;
                        IsEnabledTaskStatus = true;
                        GBTaskStatus = GetColorBrushByStatu(0);
                    }
                    RaisePropertyChanged("IblTaskContent");
                }
            }
        }

        //fefefe
        //设备连接 参数 
        private bool _isCheckedWorkStatus = false;
        private bool _isEnabledWorkStatus = true;
        private SolidColorBrush _gbWorkStatus;
        private string _lblWorkContent = "运行";
        private double _opcElementViewHeight = 50.0;

        /// <summary>
        /// 连接状态  IsChecked
        /// </summary>
        public bool IsCheckedWorkStatus
        {
            get
            {
                return _isCheckedWorkStatus;
            }
            set
            {
                if (_isCheckedWorkStatus != value)
                {
                    _isCheckedWorkStatus = value;
                    RaisePropertyChanged("IsCheckedWorkStatus");
                }
            }
        }
        /// <summary>
        /// 连接状态  IsEnabled
        /// </summary>
        public bool IsEnabledWorkStatus
        {
            get
            {
                return _isEnabledWorkStatus;
            }
            set
            {
                if (_isEnabledWorkStatus != value)
                {
                    _isEnabledWorkStatus = value;
                    RaisePropertyChanged("IsEnabledWorkStatus");
                }
            }
        }
        public SolidColorBrush GBWorkStatus
        {
            get
            {
                if (_gbWorkStatus == null)
                {
                    _gbWorkStatus = GetColorBrushByStatu(0);
                }
                return _gbWorkStatus;
            }
            set
            {
                if (_gbWorkStatus != value)
                {
                    _gbWorkStatus = value;
                    RaisePropertyChanged("GBWorkStatus");
                }
            }
        }



        /// <summary>
        /// 根据不同的状态类型 返回不同的颜色 SolidColorBrush
        /// </summary>
        /// <param name="index">0 空闲、无任务、未开始   1正常  2异常、失败  3暂停  4停止</param>
        /// <returns>SolidColorBrush 颜色对象</returns>
        private SolidColorBrush GetColorBrushByStatu(int index)
        {
            string strColorValue = "#ffffff";
            switch (index)
            {
                case 0://空闲
                    strColorValue = "#cfcfcf";
                    break;
                case 1://正常
                    strColorValue = "#5efc82";
                    break;
                case 2://异常
                    strColorValue = "#ff0000";
                    break;
                case 3://暂停
                    strColorValue = "#5efc82";
                    break;
                case 4://停止
                    strColorValue = "#cfcfcf";
                    break;
                default:
                    break;
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(strColorValue));
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
            if (attributeName.Equals("WorkState"))
            {
                RaisePropertyChanged("PackIconTaskName");
            }
        }
    }
}
