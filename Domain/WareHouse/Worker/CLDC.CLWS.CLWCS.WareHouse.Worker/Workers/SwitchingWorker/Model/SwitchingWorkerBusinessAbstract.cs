using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.DbService.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.SwitchingWorker.Model
{
    public abstract class SwitchingWorkerBusinessAbstract : WorkerBusinessAbstract
    {
        public abstract OperateResult HandleIdentifyMsg(DeviceBaseAbstract device);
        public abstract OperateResult HandleIdentifyMsg(DeviceBaseAbstract device, List<AssistantDevice> workerAssistants, string barcode);

        public virtual OperateResult HandleValueChange(DeviceBaseAbstract device, int newValue)
        {
            return OperateResult.CreateSuccessResult();
        }
    }
}
