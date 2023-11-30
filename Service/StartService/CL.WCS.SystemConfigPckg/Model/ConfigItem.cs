using System.Xml.Serialization;

namespace CL.WCS.SystemConfigPckg.Model
{
    public class ConfigItem<T>
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("Description")]
        public string Description { get; set; }
        [XmlAttribute("Value")]
        public T Value { get; set; }

    }
}
