using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Config;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Robot
{
    /// <summary>
    /// 机器人设备的配置属性
    /// </summary>
    [Serializable]
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    public class RobotDeviceProperty : DevicePropertyAbstract<CommonConfig, IdentifyBusinessHandleProperty, RobotDeviceControlHandleProperty>
    {
    }
    [Serializable]
    [XmlRoot("ControlHandle", Namespace = "", IsNullable = false)]
    public class RobotDeviceControlHandleProperty : ControlHandlePropertyAbstract<RobotDeviceProtocolTranslationProperty, RobotCommunicateProperty>
    {
        public override UserControl CreateView()
        {
            return new UserControl();
        }
    }

    [Serializable]
    public class RobotDeviceProtocolTranslationProperty :
        ProtocolTranslationPropertyAbstract<RobotDeviceProtocolTranslationConfigProperty>, IProperty
    {
        public UserControl CreateView()
        {
            return new UserControl();
        }
    }
    [Serializable]
    public class RobotDeviceProtocolTranslationConfigProperty
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }
    }

   
}
