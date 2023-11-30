using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Roller.ViewModel
{
    public class RollerDeviceViewModel : TransportDeviceViewModel
    {
        public RollerDeviceViewModel(TransportDeviceBaseAbstract device)
            : base(device)
        {
            _deviceControl = (RollerDeviceControl)Device.DeviceControl;
            OpcElementViewModel=new OpcElementViewModel();
            InitOpcElementViewModel();
        }
        public OpcElementViewModel OpcElementViewModel { get; private set; }

        private void InitOpcElementViewModel()
        {
            OpcElementViewModel.OpcElementViewHeight = DeviceOpcElement.Datablocks.Count * 50.0;
            OpcElementViewModel.DeviceOpcElement = DeviceOpcElement;
            OpcElementViewModel.RefreshAllData = _deviceControl.Communicate.RefreshAllData;
            OpcElementViewModel.Write = _deviceControl.Communicate.Write;
        }

        private readonly RollerDeviceControl _deviceControl;
        public OpcElement DeviceOpcElement
        {
                          
            get
            {
                if (_deviceControl == null)
                {
                    return new OpcElement();
                }
                return _deviceControl.Communicate.OpcElement;
            }
        }

    }
}
