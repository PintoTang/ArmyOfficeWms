using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.ViewModel
{
   public class RequestRCSPermitViewModel : ViewModelBase, IInvokeCmd
    {
        public RequestRCSPermitCmd DataModel { get; set; }

        public RequestRCSPermitViewModel(RequestRCSPermitCmd dataModel)
        {
            DataModel = dataModel;
        }

        public string GetCmd()
        {
            return DataModel.ToJson();
        }
    }
}
