using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model
{
    public class SendChangeTaskCmd
    {
        public int DEST_LAYER { get; set; }
        public string AGV_NO { get; set; }
    }
}
