using System.Windows;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel
{
    /// <summary>
    /// 设备的虚拟类
    /// </summary>
    public abstract class DeviceViewModelAbstract<TDevice> : WareHouseViewModelBase, IDoTask where TDevice : DeviceBaseAbstract
    {

        protected virtual void InitViewPanel()
        {
            IsShowCurrentStatusZone = Visibility.Visible;
            IsShowControlZone = Visibility.Collapsed;
            IsShowStatusZone = Visibility.Collapsed;
            IsShowTransportZone = Visibility.Collapsed;
            if (Device.DeviceType == DeviceTypeEnum.TransportDevice)
            {
                IsShowControlZone = Visibility.Visible;
                IsShowStatusZone = Visibility.Visible;
                IsShowTransportZone = Visibility.Visible;
            }
            if (Device.DeviceType == DeviceTypeEnum.PalletizerDevice)
            {
                IsShowTransportZone = Visibility.Visible;
            }
        }
        public Visibility IsShowCurrentStatusZone { get; set; }
        public Visibility IsShowTransportZone { get; set; }
        public Visibility IsShowControlZone { get; set; }

        public Visibility IsShowStatusZone { get; set; }

        public TDevice Device { get; set; }
        public DeviceViewModelAbstract(TDevice device)
        {
            Device = device;
            Id = Device.Id;
            Name = Device.Name;
            this.Device.RegisterAttributeListener(this);
            GroupName = device.DeviceType.ToString();
            device.NotifyMsgToUiEvent += ShowLogMessage;
            InitilizeViewPanel();
        }

        private void InitilizeViewPanel()
        {
            InitViewPanel();
        }

        private MyCommand _openAssistantCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand OpenAssistantCommand
        {
            get
            {
                if (_openAssistantCommand == null)
                    _openAssistantCommand = new MyCommand(OpenAssistantView);
                return _openAssistantCommand;
            }
        }

        public bool IsHasTask()
        {
            return Device.IsHasTask;
        }

        public bool IsHasError()
        {
            return Device.IsHasError;
        }

        private void OpenAssistantView(object obj)
        {
            Window assistantView = Device.GetAssistantView();
            if (assistantView == null)
            {
                return;
            }
            assistantView.ShowDialog();
        }


        private MyCommand _openConfigCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand OpenConfigCommand
        {
            get
            {
                if (_openConfigCommand == null)
                    _openConfigCommand = new MyCommand(OpenConfigView);
                return _openConfigCommand;
            }
        }

        private void OpenConfigView(object obj)
        {
            Window configView = Device.GetConfigView();
            if (configView == null)
            {
                return;
            }
            configView.ShowDialog();
        }



        private MyCommand _showDetailCommad;
        /// <summary>
        /// 设备开始\暂停
        /// </summary>
        public MyCommand ShowDetailCommad
        {
            get
            {
                if (_showDetailCommad == null)
                    _showDetailCommad = new MyCommand(ShowDeviceDetail);
                return _showDetailCommad;
            }
        }

        private void ShowDeviceDetail(object arg)
        {
            if (Device == null)
            {
                return;
            }
            DeviceDetailView detailView = new DeviceDetailView(Device);
            detailView.ShowDialog();
        }



    }
}
