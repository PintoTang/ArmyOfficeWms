using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model
{
    [Serializable]
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    public class OrderWorkerConfigProperty : IProperty
    {
        [XmlElement("Assistants")]
        public AssistantsProperty Devices { get; set; }

        [XmlElement("AddrPrefixs")]
        public AddrPrefixsProperty AddrPrefixs { get; set; }

        public UserControl CreateView()
        {
            return new OrderWorkerConfigView(this);
        }
    }
}
