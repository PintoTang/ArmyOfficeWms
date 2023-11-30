using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.WebApi
{
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    [Serializable]
    public class WebApiClientProperty
    {
        [XmlElement("ApiPort")]
        public int ApiPort { get; set; }

        [XmlElement("ApiIp")]
        public string ApiIp { get; set; }

    }
}
