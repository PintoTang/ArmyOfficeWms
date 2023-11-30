using System.Xml.Serialization;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.Infrastructrue.DataModel
{
    public class InstanceProperty:ViewModelBase
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Class")]
        public string ClassName { get; set; }

        [XmlAttribute("NameSpace")]
        public string NameSpace { get; set; }
    }
}
