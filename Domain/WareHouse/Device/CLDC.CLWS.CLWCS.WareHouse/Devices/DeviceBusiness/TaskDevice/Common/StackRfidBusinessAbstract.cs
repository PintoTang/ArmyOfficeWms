using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Palletizer.StackRfid
{
    public abstract class StackRfidBusinessAbstract : DeviceBusinessBaseAbstract
    {
        /// <summary>
        /// 碟盘机的容器
        /// </summary>
        public int Capacity { get; set; }
    }
}
