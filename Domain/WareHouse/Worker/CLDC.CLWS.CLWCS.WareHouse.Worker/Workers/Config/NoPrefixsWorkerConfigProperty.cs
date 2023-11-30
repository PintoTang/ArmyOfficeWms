using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config
{
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    [Serializable]
    public class NoPrefixsWorkerConfigProperty : IProperty
    {
        [XmlElement("Assistants")]
        public AssistantsProperty Devices { get; set; }
        public UserControl CreateView()
        {
            return new NoPrefixsWorkerConfigView(this);
        }
    }
}
