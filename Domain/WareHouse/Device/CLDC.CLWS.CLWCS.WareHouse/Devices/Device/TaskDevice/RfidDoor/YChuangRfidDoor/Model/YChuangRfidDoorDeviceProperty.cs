using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model.Config;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TaskDevice.RfidDoor.YChuangRfidDoor.Model
{
    [Serializable]
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    public class YChuangRfidDoorDeviceProperty : DevicePropertyAbstract<CommonConfig, YChuangRfidBusinessHandleProperty, YChuangRfidDoorControlHandleProperty>
    {
    }
    [Serializable]
    [XmlRoot("ControlHandle", Namespace = "", IsNullable = false)]
    public class YChuangRfidDoorControlHandleProperty : ControlHandlePropertyAbstract<YChuangRfidBusinessHandleProperty, WebClientCommunicationProperty>
    {
        public override UserControl CreateView()
        {
            return new UserControl();
        }
    }

    [Serializable]
    [XmlRoot("BusinessHandle", Namespace = "", IsNullable = false)]
    public class YChuangRfidBusinessHandleProperty : BusinessHandleBasicPropertyAbstract
    {

        [XmlElement("Config")]
        public YChuangRfidBusinessConfigProperty Config { get; set; }

        public override UserControl CreateView()
        {
            return new UserControl();
        }
    }

    public class YChuangRfidBusinessConfigProperty
    {
        [XmlElement("RequestAddr")]
        public string RequestAddr { get; set; }
    }


    [Serializable]
    public class YChuangRfidDoorProtocolProperty :
        ProtocolTranslationPropertyAbstract<YChuangRfidDoorProtocolTranslationConfigProperty>, IProperty
    {
        public UserControl CreateView()
        {
            return new UserControl();
        }
    }

    [Serializable]
    public class YChuangRfidDoorProtocolTranslationConfigProperty
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }
    }
}
