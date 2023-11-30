using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model
{
    /// <summary>
    /// 杭叉车 暂停和继续状态
    /// </summary>
    public enum FourWayVehicleCarRunEnum
    {
        [Description("继续")]
        ContinueRunning = 0,
        [Description("暂停")]
        Pause = 1
    }
}
