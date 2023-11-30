using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model;
using GalaSoft.MvvmLight;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.ViewModel
{
    public class SendChangeTaskCmdViewModel : ViewModelBase, IInvokeCmd
    {
        public SendChangeTaskCmd DataModel { get; set; }

        public SendChangeTaskCmdViewModel(SendChangeTaskCmd dataModel)
        {
            DataModel = dataModel;
        }

        public string GetCmd()
        {
            return DataModel.ToJson();
        }
    }
}
