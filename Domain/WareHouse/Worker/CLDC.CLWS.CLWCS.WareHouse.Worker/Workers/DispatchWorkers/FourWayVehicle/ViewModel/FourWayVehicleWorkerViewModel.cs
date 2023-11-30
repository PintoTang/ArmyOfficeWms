using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.Common;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.FourWayVehicle.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.FourWayVehicle.ViewModel
{
    public class FourWayVehicleWorkerViewModel : OrderWorkerViewModel<FourWayVehicleWorkerBusiness>
    {
        public FourWayVehicleWorkerViewModel(DispatchWorkerAbstract<FourWayVehicleWorkerBusiness> worker)
            : base(worker)
        {
           
        }
       
    }
}
