using System;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client.View;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Robot
{
    [XmlRoot("Communication", Namespace = "", IsNullable = false)]
    [Serializable]
    public class RobotCommunicateProperty : InstanceProperty, IProperty
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlElement("Config")]
        public SocketClientProperty Config { get; set; }

        public UserControl CreateView()
        {
            return new RobotCommunicateView(this);
        }
    }

}
