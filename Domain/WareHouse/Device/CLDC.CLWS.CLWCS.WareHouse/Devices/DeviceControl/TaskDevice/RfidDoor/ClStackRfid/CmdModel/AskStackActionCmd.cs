
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.ClStackRfid.CmdModel
{
    /// <summary>
    /// AskStackActionCmd 协议类
    /// </summary>
    public class AskStackActionCmd
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DEVICE_NO { get; set; }
        /// <summary>
        /// StackPutPick 类型
        /// </summary>
        public StackPutPick ACTION { get; set; }
    }

    /// <summary>
    /// 取放货枚举
    /// </summary>
    public enum StackPutPick
    {
        [Description("取货")]
        Pick = 1,
        [Description("放货")]
        Put = 2
    }

}
