using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.StockingWorker.Model
{
    public abstract class StockingWorkerBusinessAbstract : WorkerBusinessAbstract
    {
        public abstract OperateResult HandleIdentifyMsg(DeviceBaseAbstract device, List<AssistantDevice> workerAssistants, string barcode);
    }
}
