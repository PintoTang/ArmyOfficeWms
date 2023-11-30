using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel
{
    public class DeviceDelegate
    {
       public delegate void HandleOrderValueChange(DeviceName deviceName, int value);
    }
}
