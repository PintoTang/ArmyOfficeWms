using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.ViewModel
{
    public class IdentityDeviceForSocketViewModel : DeviceViewModelAbstract<IdentifyDeviceForSocket>
    {
        public IdentityDeviceForSocketViewModel(IdentifyDeviceForSocket device)
            : base(device)
        {
            _controlCommunicate = (IdentifyDeviceCommForClouRfid)Device.DeviceControl.Communicate;
            //OpcElementViewModel = new OpcElementViewModel();
            //InitOpcElementViewModel();
        }

        //public OpcElementViewModel OpcElementViewModel { get; private set; }

        private void InitOpcElementViewModel()
        {
            //OpcElementViewModel.OpcElementViewHeight = DeviceOpcElement.Datablocks.Count * 50.0;
            //OpcElementViewModel.DeviceOpcElement = DeviceOpcElement;
            //OpcElementViewModel.RefreshAllData = _controlCommunicate.PlcCommunicate.RefreshAllData;
            //OpcElementViewModel.Write = _controlCommunicate.PlcCommunicate.Write;
        }

        public RoleLevelEnum CurUserLevel
        {
            get
            {
                if (CookieService.CurSession != null)
                {
                    return CookieService.CurSession.RoleLevel;
                }
                else
                {
                    return RoleLevelEnum.游客;
                }
            }
        }
        public double OpcElementViewHeight
        {
            get { return _opcElementViewHeight; }
            set
            {
                _opcElementViewHeight = value;
                RaisePropertyChanged("OpcElementViewHeight");
            }
        }


        private readonly IdentifyDeviceCommForClouRfid _controlCommunicate;

        //public OpcElement DeviceOpcElement
        //{
        //    get
        //    {
        //        if (_controlCommunicate == null)
        //        {
        //            return new OpcElement();
        //        }
        //        return _controlCommunicate.PlcCommunicate.OpcElement;
        //    }
        //}

        public List<LiveData> CurrentLiveData
        {
            get { return Device.CurrentLiveData.Clone(); }
        }


        private MyCommand _deleteLiveDataCommand;
        public MyCommand DeleteLiveDataCommand
        {
            get
            {
                if (_deleteLiveDataCommand == null)
                {
                    _deleteLiveDataCommand = new MyCommand(DeleteLiveData);
                }
                return _deleteLiveDataCommand;
            }
        }



        private void DeleteLiveData(object obj)
        {
            if (!(obj is LiveData))
            {
                return;
            }
            MessageBoxResult result = MessageBoxEx.Show("确定删除？", "警告", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            LiveData deleteData = obj as LiveData;
            OperateResult deleteResult = Device.DeleteLiveDataContent(deleteData);
            if (deleteResult.IsSuccess)
            {
                SnackbarQueue.MessageQueue.Enqueue("删除成功");
            }
            else
            {
                MessageBoxEx.Show(string.Format("删除失败，原因：{0}", deleteResult.Message));
            }
        }
        public void ShowPackage(Package package)
        {
            if (System.Windows.Application.Current.Dispatcher != null)
                System.Windows.Application.Current.Dispatcher.Invoke(new Action<Package>(p =>
                {
                    CurrentPackage = p;
                    PackageLst.Add(p);
                }), DispatcherPriority.Background, package);
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
        private string _IdentityCurBarCode = "0";
        private double _opcElementViewHeight = 50.0;

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
            if (attributeName.Equals("CurrentLiveData"))
            {
                RaisePropertyChanged("CurrentLiveData");
            }
        }
    }
}
