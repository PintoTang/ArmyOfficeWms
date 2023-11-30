using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using MaterialDesignThemes.Wpf;

namespace CLDC.CLWCS.Service.MenuService.Config
{

    /// <summary>
    /// Webservice的配置类
    /// </summary>
    [XmlRoot("WcsMenuConfig", Namespace = "", IsNullable = false)]
    [Serializable]
    public class WcsMenuConfig
    {
        [XmlElement("WcsMenu")]
        public List<WcsMenuProperty> WcsMenuItemList { get; set; }

        [XmlAttribute("IsShowUi")]
        public bool IsShowUi { get; set; }

        [XmlAttribute("PackIconKind")]
        public PackIconKind IconKind { get; set; }
    }


    /// <summary>
    /// Webservice的配置类
    /// </summary>
    [XmlRoot("WcsMenu", Namespace = "", IsNullable = false)]
    [Serializable]
    public class WcsMenuProperty : InstanceProperty
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }

        [XmlAttribute("IsShowUi")]
        public bool IsShowUi { get; set; }

        [XmlAttribute("PackIconKind")]
        public PackIconKind IconKind { get; set; }

    }
}
