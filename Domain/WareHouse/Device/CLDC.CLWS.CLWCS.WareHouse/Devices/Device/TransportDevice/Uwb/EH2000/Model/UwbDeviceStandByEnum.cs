using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Uwb.EH2000.Model
{
   public enum UwbDeviceStandByEnum
    {
       [Description("待命中")]
       StandByOn=1,
       [Description("非待命")]
       StandByOff=2
    }

    public enum InOrOutEnum
    {
        [Description("进去区域")]
        In=1,
        [Description("离开区域")]
        Out=2,
    }
}
