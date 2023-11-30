using System;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.Model
{
    [Serializable]
    [XmlRoot("Coordination", Namespace = "", IsNullable = false)]
    public class IdentifyWorkerProperty : WorkerPropertyAbstract<NoPrefixsWorkerConfigProperty, IdentifyWorkerBusinessHandleProperty>
    {

    }
}
