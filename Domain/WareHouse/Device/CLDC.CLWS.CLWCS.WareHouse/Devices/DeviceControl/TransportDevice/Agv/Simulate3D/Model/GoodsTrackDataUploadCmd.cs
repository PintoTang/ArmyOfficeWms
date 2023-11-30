using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.Simulate3D.Model
{
    public class GoodsTrackDataUploadCmd
    {
        public string TASKCODE { get; set; }
        public string DEVICECODE { get; set; }
        public string PACKAGEBARCODE { get; set; }
        public string POSITION { get; set; }
        public string NEXTPOSITION { get; set; }

        public bool ISHAVEGOODS { get; set; }
    }
}
