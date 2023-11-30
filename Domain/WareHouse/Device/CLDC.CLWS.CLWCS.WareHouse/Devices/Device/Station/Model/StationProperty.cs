using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Station.Model
{
    /// <summary>
    /// 站台的属性配置
    /// </summary>
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    [Serializable]
    public class StationProperty : DevicePropertyAbstract<StationConfig, EmptyBusinessHandleProperty, StationControlHandleProperty>
    {

    }
    [Serializable]
    public class StationControlHandleProperty : ControlHandlePropertyAbstract<EmptyProtocolTranslationProperty, OpcCommunicationProperty>
    {
        public override UserControl CreateView()
        {
            return new EmptyControlPropertyView(this);
        }
    }
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    [Serializable]
    public class StationConfig : IProperty
    {
        [XmlElement("DeviceMode")]
        public string DeviceMode { get; set; }

        public  UserControl CreateView()
        {
            return new UserControl();
        }
    }

}
