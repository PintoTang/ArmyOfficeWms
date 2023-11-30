using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.DbService.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.PickingWorker.Model
{
    public abstract class PickingWorkerBusinessAbstract : WorkerBusinessAbstract
    {
        public abstract OperateResult HandleIdentifyMsg(DeviceBaseAbstract device, List<AssistantDevice> workerAssistants, string barcode);
    }
}
