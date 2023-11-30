using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Display.View;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Display.Model
{
    /// <summary>
    /// 显示设备的属性配置
    /// </summary>
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    [Serializable]
    public class DisplayDeviceProperty : DevicePropertyAbstract<DisplayConfigProperty, EmptyBusinessHandleProperty, DisplayDeviceControlHandleProperty>
    {
    }

    /// <summary>
    /// 碟盘机的属性配置
    /// </summary>
    [Serializable]
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    public class DisplayConfigProperty : IProperty
    {
        [XmlElement("IsNeedClearScreen")]
        public bool IsNeedClearScreen { get; set; }

        [XmlElement("ClearScreenInterval")]
        public float ClearScreenInterval { get; set; }

        [XmlElement("DefaultContent")]
        public string DefaultContent { get; set; }

        [XmlElement("DefaultTitle")]
        public string DefaultTitle { get; set; }

        public UserControl CreateView()
        {
            return new DisplayConfigView(this);
        }
    }

    public class DisplayDeviceControlHandleProperty : ControlHandlePropertyAbstract<DisplayDeviceProtocolTranslationProperty, CommunicationProperty>
    {
        public override UserControl CreateView()
        {
            return new EmptyControlPropertyView(this);
        }
    }


    public class DisplayDeviceProtocolTranslationProperty : ProtocolTranslationPropertyAbstract<DisplayDeviceProtocolTranslationConfigProperty>, IProperty
    {
        public UserControl CreateView()
        {
            return new UserControl();
        }
    }

    public class DisplayDeviceProtocolTranslationConfigProperty
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }

    }

    public class CommunicationProperty:IProperty
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }


        [XmlElement("Config")]
        public CommonConfig Config { get; set; }

        public UserControl CreateView()
        {
            return new UserControl();
        }
    }

}
