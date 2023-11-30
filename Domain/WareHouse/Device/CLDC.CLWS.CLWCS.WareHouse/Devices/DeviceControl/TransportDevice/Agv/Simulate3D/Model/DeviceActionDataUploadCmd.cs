using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.Simulate3D.Model
{
    public class DeviceActionDataUploadCmd
    {
        public string DEVICECODE { get; set; }
        public string ACTIONCODE { get; set; }
        public int ACTION_FLAG { get; set; }
     
    }
}
