using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CLDC.CLWS.CLWCS.Service.WmsView
{
    /// <summary>
    /// TaskType的配置类
    /// </summary>
    [XmlRoot("SoundLightConfig", Namespace = "", IsNullable = false)]
    public class SoundLightConfigModel
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("Command")]
        public List<Command> CommandList { get; set; }

    }

    [XmlRoot("Command", Namespace = "", IsNullable = false)]
    [Serializable]
    public class Command
    {
        [XmlAttribute("Area")]
        public string Area { get; set; }

        [XmlAttribute("Light")]
        public int Light { get; set; }

        [XmlAttribute("Sound")]
        public int Sound { get; set; }

        [XmlAttribute("SoundContent")]
        public string SoundContent { get; set; }

        [XmlAttribute("Code")]
        public string Code { get; set; }

    }
}
