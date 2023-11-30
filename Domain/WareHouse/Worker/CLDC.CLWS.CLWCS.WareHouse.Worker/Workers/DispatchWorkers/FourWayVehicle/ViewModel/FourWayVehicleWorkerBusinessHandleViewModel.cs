using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Stackingcrane.OpcStackingcrane.RobotTec.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.FourWayVehicle.Model;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.FourWayVehicle.ViewModel
{
    public class FourWayVehicleWorkerBusinessHandleViewModel
    {
        public FourWayVehicleWorkerBusinessHandleProperty DataModel { get; set; }
        public FourWayVehicleWorkerBusinessHandleViewModel(FourWayVehicleWorkerBusinessHandleProperty model)
        {
            DataModel = model;
            InitDeviceCodeLst();
            InitWebApiServiceConfigViewModel();
        }

        public WebApiServiceConfigViewModel WebApiConfigViewModel { get; set; }
        private void InitWebApiServiceConfigViewModel()
        {
            WebApiConfigViewModel=new WebApiServiceConfigViewModel();
            WebApiConfigViewModel.ControllerValue = "AgvApi";
            //WebApiConfigViewModel.PortValue = DataModel.Config.ApiPort;
            WebApiConfigViewModel.IpValue = "127.0.0.1";
        }

        private void InitDeviceCodeLst()
        {
            _deviceCodeLst.Clear();
            string tempValue = DataModel.Config.DeviceNoConvert.Trim();
            string[] key_value = tempValue.Split(';');
            foreach (string keyValue in key_value)
            {
                if (string.IsNullOrEmpty(keyValue))
                { continue; }
                string[] from_to = keyValue.Split('_');
                if (from_to.Length >= 2)
                {
                    string from = from_to[0];
                    string to = from_to[1];
                    WcsToDeviceDic dic = new WcsToDeviceDic()
                    {
                        WcsValue = from,
                        DeviceValue = to
                    };
                    _deviceCodeLst.Add(dic);
                }

            }
        }

        private ObservableCollection<WcsToDeviceDic> _deviceCodeLst = new ObservableCollection<WcsToDeviceDic>();

        public ObservableCollection<WcsToDeviceDic> DeviceCodeLst
        {
            get { return _deviceCodeLst; }
            set
            {
                _deviceCodeLst = value;
            }
        }

        private MyCommand _addDeviceCodeCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand AddDeviceCodeCommand
        {
            get
            {
                if (_addDeviceCodeCommand == null)
                    _addDeviceCodeCommand = new MyCommand(AddDeviceCode);
                return _addDeviceCodeCommand;
            }
        }
        private void AddDeviceCode(object obj)
        {
            WcsToDeviceDic item = new WcsToDeviceDic();
            DeviceCodeLst.Add(item);
        }


        private MyCommand _deleteDeviceCodeCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand DeleteDeviceCodeCommand
        {
            get
            {
                if (_deleteDeviceCodeCommand == null)
                    _deleteDeviceCodeCommand = new MyCommand(DeleteDeviceCode);
                return _deleteDeviceCodeCommand;
            }
        }

        private void DeleteDeviceCode(object arg)
        {
            if (!(arg is WcsToDeviceDic))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要删除的设备代码信息");
                return;
            }
            WcsToDeviceDic deleteItem = arg as WcsToDeviceDic;
            MessageBoxResult result = MessageBoxEx.Show("确定删除设备代码信息？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            bool removeResult = DeviceCodeLst.Remove(deleteItem);
            if (!removeResult)
            {
                string msg = "设备代码信息删除失败";
                SnackbarQueue.MessageQueue.Enqueue(msg);
            }
            else
            {
                SnackbarQueue.MessageQueue.Enqueue("设备代码信息删除成功");
            }
        }

        private MyCommand _saveDeviceCodeCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand SaveDeviceCodeCommand
        {
            get
            {
                if (_saveDeviceCodeCommand == null)
                    _saveDeviceCodeCommand = new MyCommand(SaveDeviceCode);
                return _saveDeviceCodeCommand;
            }
        }

        private void SaveDeviceCode(object arg)
        {
            StringBuilder DeviceCodeKeyValue = new StringBuilder();
            foreach (WcsToDeviceDic keyValue in DeviceCodeLst)
            {
                DeviceCodeKeyValue.Append(keyValue.WcsValue);
                DeviceCodeKeyValue.Append('_');
                DeviceCodeKeyValue.Append(keyValue.DeviceValue);
                DeviceCodeKeyValue.Append(';');
            }
            DataModel.Config.DeviceNoConvert = DeviceCodeKeyValue.ToString();
            SnackbarQueue.MessageQueue.Enqueue("设备代码信息保存成功");
        }

    }
}
