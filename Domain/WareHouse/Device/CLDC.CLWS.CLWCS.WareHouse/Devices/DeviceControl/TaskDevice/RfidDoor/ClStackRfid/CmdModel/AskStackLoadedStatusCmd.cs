using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.ClStackRfid.CmdModel
{
    public class AskStackLoadedStatusCmd
    {
        public string DEVICE_NO { get; set; }
    }

    public class StatusCmdResponse
    {
        public LoadedStatusEnumType STATUS { get; set; }
    }

    public enum LoadedStatusEnumType
    {
        [Description("满跺")]
        Full = 1,
        [Description("不满跺")]
        UnFull = 2,
        [Description("空跺")]
        Empty = 3,
        [Description("未知")]
        UnKnow=4,
    }

}
