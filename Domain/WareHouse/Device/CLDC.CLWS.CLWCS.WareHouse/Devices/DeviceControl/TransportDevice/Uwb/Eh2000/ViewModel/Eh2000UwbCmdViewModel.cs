using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Uwb.Eh2000.ViewModel
{
    public class Eh2000UwbCmdViewModel : ViewModelBase, IInvokeCmd
    {
        public string RequesValue { get; set; }
        public Eh2000UwbCmdViewModel(string requestValue)
        {
            RequesValue = requestValue;
        }
        public string GetCmd()
        {
            return RequesValue;
        }
    }
}
