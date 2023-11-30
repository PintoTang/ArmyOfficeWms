using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Config
{
   /// <summary>
    /// 设备的公共属性配置
    /// </summary>
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    [Serializable]
    public class IdentifyDeviceProperty : DevicePropertyAbstract<CommonConfig, IdentifyBusinessHandleProperty, IdentifyControlHandleProperty>
    {

    }


    [Serializable]
    [XmlRoot("BusinessHandle", Namespace = "", IsNullable = false)]
    public class IdentifyBusinessHandleProperty : BusinessHandleBasicPropertyAbstract
    {
       
        [XmlElement("Config")]
        public IdentifyBusinessConfigProperty Config { get; set; }

        public override UserControl CreateView()
        {
            //return new IdentifyBusinessPropertyView(this);
            return new UserControl();
        }
    }

    public class IdentifyBusinessConfigProperty
    {
        [XmlElement("Capacity")]
        public int Capacity { get; set; }
    }
    [Serializable]
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    public class IdentifyControlHandleProperty : ControlHandlePropertyAbstract<EmptyProtocolTranslationProperty, OpcCommunicationProperty>
    {
        public override UserControl CreateView()
        {
            return new EmptyControlPropertyView(this);
        }
    }
}
