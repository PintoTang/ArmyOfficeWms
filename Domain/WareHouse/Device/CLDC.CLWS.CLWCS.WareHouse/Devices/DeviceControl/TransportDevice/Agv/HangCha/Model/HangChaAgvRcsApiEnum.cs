using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.Model
{
    public enum HangChaAgvRcsApiEnum
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
