using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Identify
{
    /// <summary>
    /// 信息识别业务类
    /// </summary>
    public abstract class IdentifyDeviceBusinessAbstract<T> : DeviceBusinessBaseAbstract
    {
        internal abstract OperateResult<T> IdentifyMsgFilter(T identifyMsg);
        internal abstract OperateResult AfterNotifyIdetifyMsg(T identifyMsg);
        internal abstract OperateResult<T> BeforeNotifyIdetifyMsg(T identifyMsg);
    }
}
