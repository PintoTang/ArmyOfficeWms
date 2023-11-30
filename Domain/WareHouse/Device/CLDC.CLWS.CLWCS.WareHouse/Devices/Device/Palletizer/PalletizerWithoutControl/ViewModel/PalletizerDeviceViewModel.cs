using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.PalletizerWithoutControl.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Palletizer.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.ViewModel
{
    public class PalletizerDeviceViewModel : DeviceViewModelAbstract<PalletizerDeviceAbstract>
    {
        public PalletizerDeviceViewModel(PalletizerDeviceAbstract device)
            : base(device)
        {
            _palletierControl = device.DeviceControl;
            OpcElementViewModel = new OpcElementViewModel();
            PalletizerHotDataViewModel = new PalletizerHotDataViewModel(device);
            InitOpcElementViewModel();
        }

        public PalletizerHotDataViewModel PalletizerHotDataViewModel { get; private set; }

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



        public override void NotifyAttributeChange(string attributeName, object newValue)
        {
            if (attributeName.Equals("ConnectState"))
            {
                RaisePropertyChanged("IblConnContent");
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
