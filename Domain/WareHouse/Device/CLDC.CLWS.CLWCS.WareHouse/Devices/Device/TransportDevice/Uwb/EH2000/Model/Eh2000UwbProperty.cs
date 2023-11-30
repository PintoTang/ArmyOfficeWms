using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Uwb.EH2000.Model
{
    /// <summary>
    /// 恒高2000的属性配置
    /// </summary>
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    [Serializable]
    public class Eh2000UwbProperty : DevicePropertyAbstract<UwbConfig, TransportBusinessHandleProperty, Eh2000ControlHandleProperty>
    {
    }
    
    [Serializable]
    public class Eh2000ControlHandleProperty : ControlHandlePropertyAbstract<EmptyProtocolTranslationProperty, WebClientCommunicationProperty>
    {
        public override UserControl CreateView()
        {
            return new UserControl();
        }
    }

    /// <summary>
    /// 无配置
    /// </summary>
    [Serializable]
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    public class UwbConfig : IProperty
    {
        [XmlElement("ExposedUnId")]
        public string ExposedUnId { get; set; }

        /// <summary>
        /// 标签的工作区域范围
        /// </summary>
        [XmlElement("WorkArea")]
        public string WorkArea { get; set; }

        public UserControl CreateView()
        {
            return new UserControl();
        }
    }
}
