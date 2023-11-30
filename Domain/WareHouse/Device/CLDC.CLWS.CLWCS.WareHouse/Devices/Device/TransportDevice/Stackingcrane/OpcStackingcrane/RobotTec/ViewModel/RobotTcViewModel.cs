using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Stackingcrane.OpcStackingcrane.RobotTec.ViewModel
{
    public class RobotTcViewModel : TransportDeviceViewModel
    {
        public RobotTcViewModel(TransportDeviceBaseAbstract device)
            : base(device)
        {
            _deviceControl = (RobotTecStackingcraneControl)Device.DeviceControl;
            OpcElementViewModel = new OpcElementViewModel();
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

        private readonly RobotTecStackingcraneControl _deviceControl;
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
