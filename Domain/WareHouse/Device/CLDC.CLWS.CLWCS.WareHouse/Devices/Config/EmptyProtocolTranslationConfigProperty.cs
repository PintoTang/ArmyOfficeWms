using System;
using System.Xml.Serialization;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config
{
    [Serializable]
   public class EmptyProtocolTranslationConfigProperty
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }
    }
}
