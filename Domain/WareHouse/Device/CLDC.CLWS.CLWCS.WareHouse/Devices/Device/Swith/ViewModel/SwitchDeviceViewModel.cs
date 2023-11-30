using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Swith.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Swith.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Swith.ViewModel
{
    public class SwitchDeviceViewModel : DeviceViewModelAbstract<SwitchDeviceAbstract>
	{
        public SwitchDeviceViewModel(SwitchDeviceAbstract device) : base(device)
        {
            _switchDeviceControl = device.DeviceControl;
            OpcElementViewModel = new OpcElementViewModel();
            InitOpcElementViewModel();

        }

        public OpcElementViewModel OpcElementViewModel { get; private set; }
        private readonly SwitchDeviceControlAbstract _switchDeviceControl;
        private void InitOpcElementViewModel()
        {
            OpcElementViewModel.OpcElementViewHeight = DeviceOpcElement.Datablocks.Count * 50.0;
            OpcElementViewModel.DeviceOpcElement = DeviceOpcElement;
            OpcElementViewModel.RefreshAllData = _switchDeviceControl.Communicate.RefreshAllData;
            OpcElementViewModel.Write = _switchDeviceControl.Communicate.Write;
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
			
		}
	}
}
