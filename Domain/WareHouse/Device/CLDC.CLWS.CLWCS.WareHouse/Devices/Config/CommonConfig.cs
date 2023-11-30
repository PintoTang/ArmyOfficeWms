using System;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config
{
    /// <summary>
    /// 无配置
    /// </summary>
    [Serializable]
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    public class CommonConfig:IProperty
    {
        [XmlElement("ExposedUnId")]
        public string ExposedUnId { get; set; }
        public UserControl CreateView()
        {
            return new UserControl();
        }
    }
}
