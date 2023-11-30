using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config
{
    [XmlRoot("Assistants", Namespace = "", IsNullable = false)]
    [Serializable]
    public class AssistantsProperty
    {
        [XmlElement("Assistant")]
        public List<AssistantProperty> AssistantList { get; set; }
    }

    public class AssistantProperty
    {
        [XmlAttribute("DeviceId")]
        public int DeviceId { get; set; }

        [XmlAttribute("AssistantType")]
        public AssistantType AssistantType { get; set; }

        [XmlAttribute("IsRegisterFinish")]
        public bool IsRegisterFinish { get; set; }
    }

}
