using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.MNLMChaAgv.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.MNLMChaAgv.Model
{
    [Serializable]
    [XmlRoot("BusinessHandle", Namespace = "", IsNullable = false)]
    public class MNLMChaWorkerBusinessHandleProperty : BusinessHandleBasicPropertyAbstract
    {
        [XmlElement("Config")]
        public XChaWorkerBusinessConfigProperty Config { get; set; }
        public override UserControl CreateView()
        {
            return new MNLMChaWorkerBusinessConfigView(this);
        }
    }

    [Serializable]
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    public class XChaWorkerBusinessConfigProperty
    {
        [XmlElement("AssistantNoConvert")]
        public string AssistantNoConvert { get; set; }

    }

    

}
