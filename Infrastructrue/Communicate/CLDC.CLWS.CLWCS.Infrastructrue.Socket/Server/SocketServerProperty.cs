using System;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Server
{
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    [Serializable]
    public class SocketServerProperty : IProperty
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlElement("LocalPort")]
        public int LocalPort { get; set; }

        [XmlElement("LocalIp")]
        public string LocalIp { get; set; }

        [XmlElement("ProtocolType")]
        public ProtocolType ProtocolType { get; set; }

        [XmlElement("ListenCount")]
        public int ListenCount { get; set; }

        [XmlElement("MaxConcurrence")]
        public int MaxConcurrence { get; set; }

        [XmlElement("MaxConnections")]
        public int MaxConnections { get; set; }

        [XmlElement("ReceiveBufferSize")]
        public int ReceiveBufferSize { get; set; }

        public UserControl CreateView()
        {
            return new UserControl();
        }
    }
}
