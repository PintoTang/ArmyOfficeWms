using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.Model
{
    /// <summary>
    /// 栈板任务
    /// </summary>
    public class SendStackTaskCmd
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DEVICE_NO { get; set; }
        /// <summary>
        /// 任务编号
        /// </summary>
        public string TASK_NO { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public StackActionTypeEnum TASK_TYPE { get; set; }
        public int SEP_COUNT { get; set; }
        public int BOX_COUNT { get; set; }
        public BoxTypeEnum BOX_TYPE { get; set; }
    }

    public enum BoxTypeEnum
    {
        [Description("单相表")]
        SingleMeter=1,
        [Description("三相表")]
        ThreeMeter=2,
    }

    public enum StackActionTypeEnum
    {
        [Description("拆垛")]
        UnStack = 1,
        [Description("叠跺")]
        Stack = 2

    }
}
