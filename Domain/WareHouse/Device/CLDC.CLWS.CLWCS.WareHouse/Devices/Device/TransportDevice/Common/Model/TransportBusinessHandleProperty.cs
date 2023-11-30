using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model
{
    [Serializable]
    [XmlRoot("BusinessHandle", Namespace = "", IsNullable = false)]
    public class TransportBusinessHandleProperty : BusinessHandleBasicPropertyAbstract
    {
        [XmlElement("Config")]
        public TransportBusinessConfigProperty Config { get; set; }

        public override UserControl CreateView()
        {
            return new TransportBusinessPropertyView(this);
        }
    }

    [Serializable]
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    public class TransportBusinessConfigProperty
    {
        [XmlElement("StartAddress")]
        public string StartAddress { get; set; }

        [XmlElement("DestAddress")]
        public string DestAddress { get; set; }
    }

}
