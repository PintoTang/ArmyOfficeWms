using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model
{
    public enum FourWayVehicleRcsApiEnum
    {
        [Description("删除任务")]
        DeleteTask,
        [Description("查询任务信息")]
        SelectTaskInfo,
        [Description("请求RCS货物通行")]
        RequestRCSPermit,
        [Description("通知RCS货物通行完成")]
        ReportRCSPermitFinish,
        [Description("下发换层任务")]
        SendChangeTask,
        [Description("下发搬运任务")]
        SendTask,
        [Description("下发切换模式")]
        SendModeSwitch
    }
}
