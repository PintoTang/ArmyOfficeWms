using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model.Config;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TaskDevice.RfidDoor.ClStackRfid.Model
{
    [Serializable]
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    public class ClStackRfidDoorDeviceProperty : DevicePropertyAbstract<CommonConfig, ClStackRfidBusinessHandleProperty, ClRfidDoorControlHandleProperty>
    {
    }
    [Serializable]
    [XmlRoot("ControlHandle", Namespace = "", IsNullable = false)]
    public class ClRfidDoorControlHandleProperty : ControlHandlePropertyAbstract<ClRfidDoorProtocolProperty, WebClientCommunicationProperty>
    {
        public override UserControl CreateView()
        {
            return new UserControl();
        }
    }

    [Serializable]
    [XmlRoot("BusinessHandle", Namespace = "", IsNullable = false)]
    public class ClStackRfidBusinessHandleProperty : BusinessHandleBasicPropertyAbstract
    {

        [XmlElement("Config")]
        public ClStackRfidBusinessConfigProperty Config { get; set; }

        public override UserControl CreateView()
        {
            return new UserControl();
        }
    }

    public class ClStackRfidBusinessConfigProperty
    {
        [XmlElement("RequestAddr")]
        public string RequestAddr { get; set; }
    }


    [Serializable]
    public class ClRfidDoorProtocolProperty :
        ProtocolTranslationPropertyAbstract<ClRfidDoorProtocolTranslationConfigProperty>, IProperty
    {
        public UserControl CreateView()
        {
            return new UserControl();
        }
    }

    [Serializable]
    public class ClRfidDoorProtocolTranslationConfigProperty
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }
    }
}
