using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.View;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common
{
     [XmlRoot("Communication", Namespace = "", IsNullable = false)]
    [Serializable]
    public class WebClientCommunicationProperty : InstanceProperty, IProperty
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }

         [XmlAttribute("CommunicationMode")]
        public CommunicationModeEnum CommunicationMode { get; set; }

        [XmlElement("Config")]
        public WebClientCommunicationConfig Config { get; set; }

         public UserControl CreateView()
         {
             return new WebClientPropertyView(this);
         }
    }
    [Serializable]
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    public class WebClientCommunicationConfig
    {
        private int _timeOut=20;

        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlElement("Http")]
        public string Http { get; set; }

        [XmlElement("TimeOut")]
        public int TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }
    }

}
