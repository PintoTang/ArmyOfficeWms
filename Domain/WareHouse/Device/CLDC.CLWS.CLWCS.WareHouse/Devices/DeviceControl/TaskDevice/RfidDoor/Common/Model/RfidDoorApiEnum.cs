using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.Model
{
    public enum RfidDoorApiEnum
    {
        [Description("下发拆/码垛任务")]
        SendStackTask,
        [Description("下发扫描条码任务")]
        SendScanTask,
        [Description("下发开关门任务")]
        SendSwitchTask,
        [Description("下发屏显任务")]
        SendShow
    }
}
