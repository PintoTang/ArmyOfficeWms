using System;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.PalletierWorkers.Model
{
    [Serializable]
    [XmlRoot("Coordination", Namespace = "", IsNullable = false)]
    public class PalletierWorkerProperty : WorkerPropertyAbstract<NoPrefixsWorkerConfigProperty, EmptyBusinessHandleProperty>
    {

    }
}
