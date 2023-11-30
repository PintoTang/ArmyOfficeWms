using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Swith.Common
{
    public abstract class SwitchDeviceBusinessAbstract : DeviceBusinessBaseAbstract
    {
        /// <summary>
        /// 当前按钮值
        /// </summary>
        internal int CurPickingValue { get; set; }
        internal abstract bool IsNeedHandleValue(int newValue);

    }
}
