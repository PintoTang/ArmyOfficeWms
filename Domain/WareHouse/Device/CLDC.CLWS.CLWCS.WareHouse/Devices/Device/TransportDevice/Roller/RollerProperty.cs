using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Roller
{
    /// <summary>
    /// 滚筒线的属性配置
    /// </summary>
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    [Serializable]
    public class RollerProperty : DevicePropertyAbstract<CommonConfig, TransportBusinessHandleProperty, RollerControlHandleProperty>
    {
    }
    [Serializable]
    public class RollerControlHandleProperty : ControlHandlePropertyAbstract<EmptyProtocolTranslationProperty, OpcCommunicationProperty>
    {
        public override UserControl CreateView()
        {
            return new UserControl();
        }
    }

}
