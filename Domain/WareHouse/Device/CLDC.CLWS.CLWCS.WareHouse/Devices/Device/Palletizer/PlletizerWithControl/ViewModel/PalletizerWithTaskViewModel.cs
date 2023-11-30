using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.DataModel.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Palletizer.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.PlletizerWithControl.ViewModel
{
    public sealed class PalletizerWithTaskViewModel : DeviceViewModelAbstract<PalletizerWithTask>
    {
        public PalletizerWithTaskViewModel(PalletizerWithTask device) : base(device)
        {
            _palletierControl = device.DeviceControl;
            OpcElementViewModel = new OpcElementViewModel();
            Device = device;
            InitTaskViewModel();
            InitOpcElementViewModel();
        }

  
        public StringCharTaskViewModel TaskViewModel { get; set; }
        public OpcElementViewModel OpcElementViewModel { get; private set; }

        private PalletierControlAbstract _palletierControl;
        private void InitOpcElementViewModel()
        {
            OpcElementViewModel.OpcElementViewHeight = DeviceOpcElement.Datablocks.Count * 50.0;
            OpcElementViewModel.DeviceOpcElement = DeviceOpcElement;
            OpcElementViewModel.RefreshAllData = _palletierControl.Communicate.RefreshAllData;
            OpcElementViewModel.Write = _palletierControl.Communicate.Write;
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
        private void InitTaskViewModel()
        {
            TaskViewModel = new StringCharTaskViewModel();
            TaskViewModel.UnFinishTaskList = Device.UnFinishedTask.DataPool;
            TaskViewModel.FinishTask = Device.FinishTask;
            TaskViewModel.DoTask = Device.DoTask;
        }

        public override void NotifyAttributeChange(string attributeName, object newValue)
        {
            return;
        }
    }
}
