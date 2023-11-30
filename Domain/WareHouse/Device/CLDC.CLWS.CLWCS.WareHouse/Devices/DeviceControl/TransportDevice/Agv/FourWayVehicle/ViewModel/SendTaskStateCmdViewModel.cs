using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.ViewModel
{
    public class SendTaskStateCmdViewModel: ViewModelBase, IInvokeCmd
    {
        public SendTaskStateCmd DataModel { get; set; }

        public SendTaskStateCmdViewModel(SendTaskStateCmd dataModel)
        {
            DataModel = dataModel;
        }
      
        public string GetCmd()
        {
            return DataModel.ToString();
        }
    }
}