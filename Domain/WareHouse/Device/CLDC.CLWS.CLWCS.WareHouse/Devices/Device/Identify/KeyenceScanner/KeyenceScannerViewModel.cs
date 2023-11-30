using System;
using System.Collections.Generic;
using System.Windows;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.KeyenceScanner
{
    public class KeyenceScannerViewModel : DeviceViewModelAbstract<WareHouse.Device.KeyenceScanner>
    {
        public KeyenceScannerViewModel(WareHouse.Device.KeyenceScanner device) : base(device)
        {
            _controlCommunicate = (KeyenceScanner2000W)device.DeviceControl.Communicate;
            InitSocketViewModel();
        }

        private void InitSocketViewModel()
        {
            if (_controlCommunicate==null)
            {
                return;
            }
            SocketViewModel=new SocketClientViewModel(_controlCommunicate.SocketClient);
        }

        private readonly KeyenceScanner2000W _controlCommunicate;

        public SocketClientViewModel SocketViewModel { get; set; }

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
        public override void NotifyAttributeChange(string attributeName, object newValue)
        {         
            if (attributeName.Equals("CurrentLiveData"))
            {
                RaisePropertyChanged("CurrentLiveData");
            }
        }
    }
}
