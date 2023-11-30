using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client.View;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.Infrastructrue.Sockets
{
    [XmlRoot("Communication", Namespace = "", IsNullable = false)]
    [Serializable]
    public class SocketCommunicationProperty : InstanceProperty, IProperty
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }


        [XmlElement("Config")]
        public SocketCommunicationConfig Config { get; set; }

        public UserControl CreateView()
        {
            return new UserControl();
        }
    }
    [Serializable]
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    public class SocketCommunicationConfig
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlElement("IsServer")]
        public string IsServer { get; set; }


        [XmlElement("LocalIp")]
        public string LocalIP { get; set; }


        [XmlElement("LocalPort")]
        public string LocalPort { get; set; }


        [XmlElement("RemoteIp")]
        public string RemoteIp { get; set; }

        [XmlElement("RemotePort")]
        public string RemotePort { get; set; }
    }

}
