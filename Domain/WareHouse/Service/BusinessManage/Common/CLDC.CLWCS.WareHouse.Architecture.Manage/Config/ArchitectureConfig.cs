using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using MaterialDesignThemes.Wpf;

namespace CLDC.CLWCS.WareHouse.Architecture.Manage.Config
{

    /// <summary>
    /// Webservice的配置类
    /// </summary>
    [XmlRoot("WcsArchitectureConfig", Namespace = "", IsNullable = false)]
    [Serializable]
    public class ArchitectureConfig
    {
        [XmlElement("ArchitectureData")]
        public List<ArchitectureProperty> ArchitectureItemList { get; set; }

        [XmlAttribute("IsShowUi")]
        public bool IsShowUi { get; set; }

        [XmlAttribute("PackIconKind")]
        public PackIconKind IconKind { get; set; }
    }


    /// <summary>
    /// Webservice的配置类
    /// </summary>
    [XmlRoot("ArchitectureData", Namespace = "", IsNullable = false)]
    [Serializable]
    public class ArchitectureProperty : InstanceProperty
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }

        [XmlAttribute("IsShowUi")]
        public bool IsShowUi { get; set; }

        [XmlAttribute("PackIconKind")]
        public PackIconKind IconKind { get; set; }

    }
}
