using System;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.HangChaAgv.Model
{
    [Serializable]
    [XmlRoot("Coordination", Namespace = "", IsNullable = false)]
    public class HangChaDispatchWorkerProperty : WorkerPropertyAbstract<OrderWorkerConfigProperty, HangChaWorkerBusinessHandleProperty>
    {

    }
}
