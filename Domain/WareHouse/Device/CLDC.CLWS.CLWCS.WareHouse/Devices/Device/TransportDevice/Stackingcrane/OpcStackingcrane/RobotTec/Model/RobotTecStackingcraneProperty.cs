using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Stackingcrane.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Stackingcrane.OpcStackingcrane.RobotTec.Model
{
    /// <summary>
    /// RobotTec属性配置
    /// </summary>
    [Serializable]
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    public class RobotTecStackingcraneProperty : DevicePropertyAbstract<CommonConfig, TransportBusinessHandleProperty, RobotTecStackingcraneControlHandleProperty>
    {

    }

    [Serializable]
    public class RobotTecStackingcraneControlHandleProperty : ControlHandlePropertyAbstract<StackingcraneProtocolTranslationProperty, OpcCommunicationProperty>
    {
        public override UserControl CreateView()
        {
            return new EmptyControlPropertyView(this);
        }
    }


    [Serializable]
    [XmlRoot("ProtocolTranslation", Namespace = "", IsNullable = false)]
    public class StackingcraneProtocolTranslationProperty : ProtocolTranslationPropertyAbstract<StackingcraneProtocolTranslationConfigProperty>, IProperty
    {
        public UserControl CreateView()
        {
            return new StackingcraneProtocolTranslationView(this);
        }
    }

    [Serializable]
    public class StackingcraneProtocolTranslationConfigProperty
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlElement("IsDoubleCell")]
        public bool IsDoubleCell { get; set; }

        [XmlElement("RowChange")]
        public string RowChange { get; set; }

        [XmlElement("FaultCode")]
        public string FaultCode { get; set; }

    }

}
