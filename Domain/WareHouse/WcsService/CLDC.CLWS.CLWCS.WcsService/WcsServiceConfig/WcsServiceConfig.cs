using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using MaterialDesignThemes.Wpf;

namespace CLDC.CLWS.CLWCS.WcsService.WcsServiceConfig
{

    /// <summary>
    /// Webservice的配置类
    /// </summary>
    [XmlRoot("WcsServiceConfig", Namespace = "", IsNullable = false)]
    [Serializable]
    public class WcsServiceConfig
    {
        [XmlElement("WcsService")]
        public List<WcsServiceProperty> WebserviceList { get; set; }
    }


    /// <summary>
    /// Webservice的配置类
    /// </summary>
    [XmlRoot("WcsService", Namespace = "", IsNullable = false)]
    [Serializable]
    public class WcsServiceProperty : InstanceProperty
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }

        [XmlAttribute("ServiceType")]
        public string ServiceType { get; set; }
        [XmlAttribute("IsShowUi")]
        public bool IsShowUi { get; set; }

        [XmlAttribute("PackIconKind")]
        public PackIconKind IconKind { get; set; }

    }
}
