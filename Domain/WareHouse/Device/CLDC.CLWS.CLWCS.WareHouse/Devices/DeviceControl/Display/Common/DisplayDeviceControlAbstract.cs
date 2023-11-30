using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Display.Common
{
    public abstract class DisplayDeviceControlAbstract:DeviceControlBaseAbstract
    {
        internal string TitleContent { get; set; }
        internal abstract OperateResult ClearScreen();
        internal abstract OperateResult SendContent(string content);
        internal abstract OperateResult SendTitle(string title);
    }
}
