using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.ClStackRfid.CmdModel
{
    public class SendStackDispatchCmd
    {
        public string DEVICE_NO { get; set; }
        public string TASK_NO { get; set; }
        public StackPositionEnum FROM_POSITION { get; set; }
        public StackPositionEnum TO_POSITION { get; set; }
    }


    public enum StackPositionEnum
    {
        [Description("射频门内")]
        StackDevice=1,
        [Description("栈板车缓存")]
        StackBufferDevice=2,
    }

}
