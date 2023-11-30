using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.Model;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.ViewModel
{
   public class SendNextAddrPermitCmdViewModel:ViewModelBase, IInvokeCmd
    {
        public SendNextAddrPermitCmd DataModel { get; set; }

        public SendNextAddrPermitCmdViewModel(SendNextAddrPermitCmd dataModel)
        {
            DataModel = dataModel;
        }

        public string GetCmd()
        {
            return DataModel.ToJson();
        }
    }
}
