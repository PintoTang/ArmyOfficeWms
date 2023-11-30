using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.FourWayVehicle.AgvApi
{

    public interface IFourWayVehicleApi
    {
        OperateResult NotifyDeviceStatus(string cmd);

        OperateResult NotifyExeResult(string cmd);
    }
}
