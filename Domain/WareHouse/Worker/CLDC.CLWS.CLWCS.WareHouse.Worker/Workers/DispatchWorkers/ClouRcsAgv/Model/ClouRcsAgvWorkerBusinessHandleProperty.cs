using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.ClouRcsAgv.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.ClouRcsAgv.Model
{
    [Serializable]
    [XmlRoot("BusinessHandle", Namespace = "", IsNullable = false)]
    public class ClouRcsAgvWorkerBusinessHandleProperty : BusinessHandleBasicPropertyAbstract
    {
        [XmlElement("Config")]
        public ClouRcsAgvWorkerBusinessConfigProperty Config { get; set; }
        public override UserControl CreateView()
        {
            return new ClouRcsAgvWorkerBusinessConfigView(this);
        }
    }

    [Serializable]
    [XmlRoot("Config", Namespace = "", IsNullable = false)]
    public class ClouRcsAgvWorkerBusinessConfigProperty
    {
        [XmlElement("DeviceNoConvert")]
        public string DeviceNoConvert { get; set; }

    }

    

}
