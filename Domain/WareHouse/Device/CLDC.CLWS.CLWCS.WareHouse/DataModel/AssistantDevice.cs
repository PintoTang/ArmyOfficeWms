using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel
{
    /// <summary>
    /// 协助者
    /// </summary>
    public class AssistantDevice
    {
        public AssistantDevice(DeviceBaseAbstract device,AssistantType type= AssistantType.Station,bool isRegisterFinish=false)
        {
            Device = device;
            AssistantType = type;
            IsRegisterFinish = isRegisterFinish;
        }
        public DeviceBaseAbstract Device { get; set; }
        public AssistantType AssistantType { get; set; }

        public bool IsRegisterFinish { get; set; }

        public int Id { get { return Device.Id; } }

    }

    /// <summary>
    /// 协助者的类型
    /// </summary>
    public enum AssistantType
    {
        [Description("站点")]
        Station = 0,
        [Description("搬运")]
        Transport = 1,
        [Description("就绪点")]
        ReadyPoint=2,
        [Description("外容器扫描设备")]
        ContainerScanner = 3,
        [Description("内物扫描设备")]
        ContentScanner=4,
        [Description("显屏设备")]
        Display=5,
        [Description("拆码盘机")]
        Palletizer=6,
        [Description("切换设备")]
        SwitchDevice = 7

    }
}
