using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.ClStackRfid.CmdModel
{
    public enum ClStackRfidDoorApiEnum
    {
        [Description("下发拆/码垛任务")]
        SendStackTask,
        [Description("下发扫描条码任务")]
        SendScanTask,
        [Description("下发开关门任务")]
        SendSwitchTask,
        [Description("下发屏显任务")]
        SendShow,
        [Description("查询栈板车缓存状态")]
        AskStackLoadedStatus,
        [Description("申请栈板车专机取/放")]
        AskStackAction,
        [Description("下发调度栈板车任务")]
        SendStackDispatch
    }
}
