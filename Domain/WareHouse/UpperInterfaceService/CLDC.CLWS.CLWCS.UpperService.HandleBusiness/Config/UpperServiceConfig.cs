using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.UpperService.HandleBusiness.Config
{
    /// <summary>
    /// Webservice的配置类
    /// </summary>
    [XmlRoot("UpperServiceConfig", Namespace = "", IsNullable = false)]
    [Serializable]
    public class UpperServiceConfig
    {
        [XmlElement("UpperService")]
        public List<UpperServiceProperty> UpperServiceList { get; set; } 
    }


    /// <summary>
    /// Webservice的配置类
    /// </summary>
    [XmlRoot("UpperService", Namespace = "", IsNullable = false)]
    [Serializable]
    public class UpperServiceProperty : InstanceProperty
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }

        [XmlAttribute("SystemName")]
        public UpperSystemEnum SystemName { get; set; }
    }

}
