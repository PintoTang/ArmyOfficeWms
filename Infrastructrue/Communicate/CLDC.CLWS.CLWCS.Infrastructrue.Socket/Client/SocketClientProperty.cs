using System;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client.View;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client
{
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    [Serializable]
    public class SocketClientProperty
    {
        [XmlElement("LocalIp")]
        public string LocalIp { get; set; }

        [XmlElement("ProtocolType")]
        public ProtocolType ProtocolType { get; set; }

        [XmlElement("LocalPort")]
        public int LocalPort { get; set; }

        [XmlElement("RemoteIp")]
        public string RemoteIp { get; set; }

        [XmlElement("RemotePort")]
        public int RemotePort { get; set; }
    }

}
