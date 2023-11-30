using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.Model
{
    /// <summary>
    /// 杭叉车 暂停和继续状态
    /// </summary>
    public enum HangChaAgvCarRunEnum
    {
        [Description("继续")]
        ContinueRunning = 0,
        [Description("暂停")]
        Pause = 1
    }
}
