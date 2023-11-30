using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model.Config
{
    /// <summary>
    /// 设备的公共属性配置
    /// </summary>
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    [Serializable]
    public class PalletizerDeviceProperty : DevicePropertyAbstract<CommonConfig, PalletizerBusinessHandleProperty, PalletizerControlHandleProperty>
    {

    }


    [Serializable]
    [XmlRoot("BusinessHandle", Namespace = "", IsNullable = false)]
    public class PalletizerBusinessHandleProperty : BusinessHandleBasicPropertyAbstract
    {
       
        [XmlElement("Config")]
        public PalletizerBusinessConfigProperty Config { get; set; }

        public override UserControl CreateView()
        {
            return new PalletizerBusinessPropertyView(this);
        }
    }

    public class PalletizerBusinessConfigProperty
    {
        [XmlElement("Capacity")]
        public int Capacity { get; set; }

        [XmlElement("IsNeedHandleEachFinish")]
        public bool IsNeedHandleEachFinish { get; set; }

        [XmlElement("IsNeedVerifyCapacity")]
        public bool IsNeedVerifyCapacity { get; set; }
    }
    [Serializable]
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    public class PalletizerControlHandleProperty : ControlHandlePropertyAbstract<EmptyProtocolTranslationProperty, OpcCommunicationProperty>
    {
        public override UserControl CreateView()
        {
            return new EmptyControlPropertyView(this);
        }
    }

}
