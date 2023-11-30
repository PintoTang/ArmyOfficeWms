using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.HangChaAgv.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.HangChaAgv.Model
{
    [Serializable]
    [XmlRoot("BusinessHandle", Namespace = "", IsNullable = false)]
    public class HangChaWorkerBusinessHandleProperty : BusinessHandleBasicPropertyAbstract
    {
        [XmlElement("Config")]
        public HangChaWorkerBusinessConfigProperty Config { get; set; }
        public override UserControl CreateView()
        {
            return new HangChaWorkerBusinessConfigView(this);
        }
    }

    [Serializable]
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    public class HangChaWorkerBusinessConfigProperty
    {
        [XmlElement("DeviceNoConvert")]
        public string DeviceNoConvert { get; set; }

    }

    

}
