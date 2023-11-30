using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.HangChaAgv.AgvApi
{

    public interface IHangChaAgvApi
    {
        OperateResult NotifyDeviceStatus(string cmd);

        OperateResult NotifyExeResult(string cmd);
    }
}
