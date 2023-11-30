using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config
{
    [Serializable]
    [XmlRoot("AddrPrefixs", Namespace = "", IsNullable = false)]
   public class AddrPrefixsProperty
    {
        [XmlElement("Prefixs")]
        public List<PrefixsItem> AddrPrefixsList { get; set; }
    }

    public class PrefixsItem
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }
        [XmlAttribute("Value")]
        public string Value { get; set; }
    }

}
