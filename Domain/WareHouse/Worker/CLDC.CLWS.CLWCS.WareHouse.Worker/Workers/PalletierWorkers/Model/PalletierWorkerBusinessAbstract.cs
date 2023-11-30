using System.Collections.Generic;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.DbService.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model.Config;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.PalletierWorkers.Model
{
    public abstract class PalletierWorkerBusinessAbstract : WorkerBusinessAbstract
    {
        public abstract OperateResult PalletierFinishHandle(DeviceBaseAbstract device, int count, List<PalletizeContent> content );
        public abstract OperateResult NotifyOutbound(DeviceBaseAbstract device);
        public abstract OperateResult PalletizerFinishEachHandle(DeviceBaseAbstract device, int count, List<PalletizeContent> content);

    }
}
