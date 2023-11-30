using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.FourWayVehicle.Model
{
    public enum FourWayVehicleApiEnum
    {
        [Description("请求下发任务")]
        addTask,
        [Description("查询任务状态")]
        taskState,
        [Description("查询车辆状态")]
        carInfo,
        [Description("删除任务")]
        deleteTask,
        [Description("车辆暂停与启动")]
        carOperationControl
    }
}
