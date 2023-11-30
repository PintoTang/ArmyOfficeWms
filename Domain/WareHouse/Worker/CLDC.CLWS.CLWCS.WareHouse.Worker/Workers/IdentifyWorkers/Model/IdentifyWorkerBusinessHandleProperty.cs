using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.Model
{
    [Serializable]
    [XmlRoot("BusinessHandle", Namespace = "", IsNullable = false)]

    public class IdentifyWorkerBusinessHandleProperty : BusinessHandleBasicPropertyAbstract
    {
        [XmlElement("Config")]
        public IdentifyWorkerBusinessConfigProperty Config { get; set; }
        public override UserControl CreateView()
        {
            return new IdentifyWorkerBusinessConfigView(this);
        }
    }
}
