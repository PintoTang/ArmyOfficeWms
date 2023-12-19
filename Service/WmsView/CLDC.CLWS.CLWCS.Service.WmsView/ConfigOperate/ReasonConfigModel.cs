using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CLDC.CLWS.CLWCS.Service.WmsView
{
    /// <summary>
    /// TaskType的配置类
    /// </summary>
    [XmlRoot("ReasonConfig", Namespace = "", IsNullable = false)]
    public class ReasonConfigModel
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("Reason")]
        public List<Reason> ReasonList { get; set; }

    }

    [XmlRoot("Reason", Namespace = "", IsNullable = false)]
    [Serializable]
    public class Reason
    {
        [XmlAttribute("Id")]
        public string Id { get; set; }

        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

    }
}
