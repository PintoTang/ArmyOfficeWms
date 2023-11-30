using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.ViewModel
{
    public class AssistantDeviceViewModel
    {
        private AssistantsProperty DevicesData { get; set; }
        public AssistantDeviceViewModel(AssistantsProperty devicesProperty)
        {
            DevicesData = devicesProperty;
            InitDeviceLst();
        }
        public ObservableCollection<DeviceBaseAbstract> DeviceLst { get; set; }

        private void InitDeviceLst()
        {
            DeviceLst = new ObservableCollection<DeviceBaseAbstract>();
            foreach (AssistantProperty deviceProperty in DevicesData.AssistantList)
            {
               DeviceBaseAbstract device= DeviceManage.Instance.FindDeivceByDeviceId(deviceProperty.DeviceId);
                if (device!=null)
                {
                    DeviceLst.Add(device);
                }
            }
        }

        private MyCommand _openAssistantCommand;

        public MyCommand OpenAssistantCommand
        {
            get
            {
                if (_openAssistantCommand == null)
                    _openAssistantCommand = new MyCommand(OpenAssistant);
                return _openAssistantCommand;
            }
        }

        private void OpenAssistant(object arg)
        {
            DeviceBaseAbstract device = arg as DeviceBaseAbstract;
            if (device != null)
            {
                DeviceBaseAbstract selectDevice= device;
                DeviceDetailView detailView = new DeviceDetailView(selectDevice);
                detailView.ShowDialog();
            }
            else
            {
                return;
            }
        }


        private MyCommand _addDevicesCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand AddDevicesCommand
        {
            get
            {
                if (_addDevicesCommand == null)
                    _addDevicesCommand = new MyCommand(AddDevices);
                return _addDevicesCommand;
            }
        }
        private void AddDevices(object obj)
        {
            LoadPlace newDevice = new LoadPlace();
            newDevice.Id = 0;
            DeviceLst.Add(newDevice);
        }


        private MyCommand _deleteDevicesCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand DeleteDevicesCommand
        {
            get
            {
                if (_deleteDevicesCommand == null)
                    _deleteDevicesCommand = new MyCommand(DeleteDevices);
                return _deleteDevicesCommand;
            }
        }

        private void DeleteDevices(object arg)
        {
            if (!(arg is DeviceBaseAbstract))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要删除的协助设备");
                return;
            }
            DeviceBaseAbstract deleteItem = arg as DeviceBaseAbstract;
            MessageBoxResult result = MessageBoxEx.Show("确定删除该协助设备？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            bool removeResult = DeviceLst.Remove(deleteItem);
            if (!removeResult)
            {
                string msg = "协助设备删除失败";
                SnackbarQueue.MessageQueue.Enqueue(msg);
            }
            else
            {
                SnackbarQueue.MessageQueue.Enqueue("协助设备删除成功");
            }
        }

        private MyCommand _saveDevicesCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand SaveDevicesCommand
        {
            get
            {
                if (_saveDevicesCommand == null)
                    _saveDevicesCommand = new MyCommand(SaveDevices);
                return _saveDevicesCommand;
            }
        }

        private void SaveDevices(object arg)
        {
            DevicesData.AssistantList.Clear();
            foreach (DeviceBaseAbstract device in DeviceLst)
            {
                AssistantProperty deviceProperty = new AssistantProperty
                {
                    DeviceId = device.Id
                };
                DevicesData.AssistantList.Add(deviceProperty);
            }
            SnackbarQueue.MessageQueue.Enqueue("协助设备保存成功");
        }



    }
}
