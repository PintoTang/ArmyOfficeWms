using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.Model
{
   public abstract class InCheckWorkerAbstractForT<T>: WorkerBaseAbstract
    {
        public abstract OperateResult HandleIdentifyMsg(DeviceBaseAbstract device, T identifyMsg, params object[] para);

       public abstract void HandleIdentifyReady(DeviceName deviceName, int newValue);
    }
}
