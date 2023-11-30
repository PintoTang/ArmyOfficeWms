using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TaskDevice.RfidDoor.ClRfidDoor
{
    public enum ClouRfidDoorApiEnum
    {
        [Description("下发扫描条码任务")]
        SendScanTask,
        [Description("下发开关门任务")]
        SendSwitchTask,
        [Description("下发屏显任务")]
        SendShow,
    }
}
