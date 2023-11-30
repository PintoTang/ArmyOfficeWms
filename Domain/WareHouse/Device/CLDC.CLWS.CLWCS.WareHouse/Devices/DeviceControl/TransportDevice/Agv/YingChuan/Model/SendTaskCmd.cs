using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.Model
{
   public class SendTaskCmd
    {
       public string TASK_NO { get; set; }
       public int TASK_TYPE { get; set; }
       public int PRI { get; set; }
       public string PACKAGE_BARCODE { get; set; }
       public string START_ADDR { get; set; }
       public string END_ADDR { get; set; }
       public RequestTypeEnum ADDR_REQUEST { get; set; }
       public string EXT1 { get; set; }
       public string EXT2 { get; set; }
    }

    public enum RequestTypeEnum
    {
        [Description("不请求")]
        UnRequest=0,
        [Description("请求")]
        Request=1
    }

}
