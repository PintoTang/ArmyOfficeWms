using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model
{
    public enum FourWayVehicleMoveStepEnum
    {
        [Description("未执行")]
        UnExecute = 0,
        [Description("执行中")]
        Executing = 1,
        [Description("取货完成")]
        PickFinish = 2,
        [Description("带货行走")]
        MoveWithLoaded = 3,
        [Description("放货完成")]
        PutFinish = 4,
        [Description("任务完成")]
        Finish = 5,
        [Description("任务异常")]
        Exception = 6,
        [Description("任务暂停")]
        Pause = 7
    }
}
