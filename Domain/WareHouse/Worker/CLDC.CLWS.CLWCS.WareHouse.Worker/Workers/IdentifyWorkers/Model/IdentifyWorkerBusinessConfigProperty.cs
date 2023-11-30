using System;
using System.Xml.Serialization;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.Model
{
    [Serializable]
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    public class IdentifyWorkerBusinessConfigProperty
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }


        [XmlElement("Routes")]
        public RoutesProperty Routes { get; set; }

    }
    [Serializable]
    [XmlRoot("Routes", Namespace = "", IsNullable = false)]
    public class RoutesProperty
    {
        [XmlElement("PassedDevice")]
        public string PassedDevice { get; set; }

        [XmlElement("FailedDevice")]
        public string FailedDevice { get; set; }
    }

}
