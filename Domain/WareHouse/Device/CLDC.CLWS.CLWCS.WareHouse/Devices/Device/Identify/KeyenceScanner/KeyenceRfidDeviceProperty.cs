using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Config;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.KeyenceScanner
{
    /// <summary>
    /// 基恩士RFID扫描设备的配置属性
    /// </summary>
    [Serializable]
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    public class KeyenceRfidDeviceProperty: DevicePropertyAbstract<CommonConfig, IdentifyBusinessHandleProperty, KeyenceRfidDeviceControlHandleProperty>
    {
    }
    [Serializable]
    [XmlRoot("ControlHandle", Namespace = "", IsNullable = false)]
    public class KeyenceRfidDeviceControlHandleProperty : ControlHandlePropertyAbstract<KeyenceRfidDeviceProtocolTranslationProperty, KeyenceCommunicateProperty>
    {
        public override UserControl CreateView()
        {
            return new UserControl();
        }
    }

    [Serializable]
    public class KeyenceRfidDeviceProtocolTranslationProperty :
        ProtocolTranslationPropertyAbstract<KeyenceRfidDeviceProtocolTranslationConfigProperty>, IProperty
    {
        public UserControl CreateView()
        {
            return new UserControl();
        }
    }
    [Serializable]
    public class KeyenceRfidDeviceProtocolTranslationConfigProperty
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }
    }

   
}
