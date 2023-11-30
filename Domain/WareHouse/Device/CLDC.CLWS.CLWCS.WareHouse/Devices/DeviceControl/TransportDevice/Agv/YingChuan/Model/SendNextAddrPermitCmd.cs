using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.Model
{
   public class SendNextAddrPermitCmd
    {
       public string TASK_NO { get; set; }
       public string AGV_NO { get; set; }
       public string NEXT_ADDR { get; set; }
    }
}
