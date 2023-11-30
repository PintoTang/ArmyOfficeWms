using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.Simulate3D.Model
{
   public class DeviceStautsDataUploadCmd
    {
       public string DEVICECODE { get; set; }
       public int ONLINE_STATUS { get; set; }
       public int JOB_STATUS { get; set; }
       public int EXCEPTION_STATUS { get; set; }
       public string EXCEPTION_CODE { get; set; }
       public string EXCEPTION_MSG { get; set; }
       public string EXT1 { get; set; }
       public string EXT2 { get; set; }
    }
}
