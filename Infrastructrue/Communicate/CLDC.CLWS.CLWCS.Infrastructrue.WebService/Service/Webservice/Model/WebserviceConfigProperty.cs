using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.Webservice.Model
{
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    [Serializable]
    public class WebserviceConfigProperty
    {
        [XmlElement("WebserviceUrl")]
        public string WebserviceUrl { get; set; }
    }
}
