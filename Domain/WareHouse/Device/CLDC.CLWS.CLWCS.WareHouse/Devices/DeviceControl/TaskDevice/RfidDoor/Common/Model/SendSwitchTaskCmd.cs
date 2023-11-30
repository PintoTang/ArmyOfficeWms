using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.Model
{
   public class SendSwitchTaskCmd
    {
       public string TASK_NO { get; set; }
       public DoorActionTypeEnum TASK_TYPE { get; set; }
       public string DEVICE_NO { get; set; }
       public string AGV_NO { get; set; }
    }

    public enum DoorActionTypeEnum
    {
        [Description("开门")]
        OpenDoor=1,
        [Description("关门")]
        CloseDoor=2
    }
}
