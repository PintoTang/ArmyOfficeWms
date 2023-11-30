using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.Model
{
   public class SendShowCmd
    {
       public string DEVICE_NO { get; set; }
       public string MESSAGE { get; set; }
       public MessageLevelTypeEnum LEVEL { get; set; }
       public string VOICE { get; set; }
    }

    public enum MessageLevelTypeEnum
    {
        [Description("正常通知")]
        Nomal = 1,
        [Description("异常信息")]
        Exception = 2
    }
}
