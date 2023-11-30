using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.DbService.Model
{
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    [Serializable]
   public class DbServiceConfigProperty
    {
       [XmlElement("MonitorInterval")]
       public int MonitorInterval { get; set; }

       [XmlElement("SelectSql")]
       public string SelectSql { get; set; }

       [XmlElement("UpdateSql")]
       public string UpdateSql { get; set; }
    }
}
