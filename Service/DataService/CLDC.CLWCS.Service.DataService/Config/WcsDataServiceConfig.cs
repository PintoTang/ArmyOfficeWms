using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using MaterialDesignThemes.Wpf;

namespace CLDC.CLWCS.Service.DataService.Config
{

    /// <summary>
    /// Webservice的配置类
    /// </summary>
    [XmlRoot("WcsDataServiceConfig", Namespace = "", IsNullable = false)]
    [Serializable]
    public class WcsDataServiceConfig
    {
        [XmlElement("DataService")]
        public List<WcsDataServiceProperty> WcsDataServiceItemList { get; set; }

        [XmlAttribute("IsShowUi")]
        public bool IsShowUi { get; set; }

        [XmlAttribute("PackIconKind")]
        public PackIconKind IconKind { get; set; }
    }


    /// <summary>
    /// Webservice的配置类
    /// </summary>
    [XmlRoot("DataService", Namespace = "", IsNullable = false)]
    [Serializable]
    public class WcsDataServiceProperty : InstanceProperty
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }

        [XmlAttribute("IsShowUi")]
        public bool IsShowUi { get; set; }

        [XmlAttribute("PackIconKind")]
        public PackIconKind IconKind { get; set; }

    }
}
