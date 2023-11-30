using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.Model
{
    public enum YingChuangAgvRcsApiEnum
    {
        [Description("删除任务")]
        DeleteTask,
        [Description("查询任务")]
        SelectTaskInfo,
        [Description("请求设备动作")]
        SendDeviceAction,
        [Description("下发地址许可")]
        SendNextAddrPermit,
        [Description("下发任务")]
        SendTask
    }
}
