using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.Model
{
    public class SendDeviceActionCmd
    {
        public string AGV_NO { get; set; }
        public ActionTypeEnum ACTION { get; set; }
        public string TASK_NO { get; set; }
    }

    public enum ActionTypeEnum
    {
        [Description("开门")]
        OpenDoor = 1,
        [Description("关门")]
        CloseDoor=2
    }
}
