using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.FourWayVehicle.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.FourWayVehicle.Model
{
    [Serializable]
    [XmlRoot("BusinessHandle", Namespace = "", IsNullable = false)]
    public class FourWayVehicleWorkerBusinessHandleProperty : BusinessHandleBasicPropertyAbstract
    {
        [XmlElement("Config")]
        public FourWayVehicleWorkerBusinessConfigProperty Config { get; set; }
        public override UserControl CreateView()
        {
            return new FourWayVehicleWorkerBusinessConfigView(this);
        }
    }

    [Serializable]
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    public class FourWayVehicleWorkerBusinessConfigProperty
    {
        [XmlElement("DeviceNoConvert")]
        public string DeviceNoConvert { get; set; }

    }

    

}
