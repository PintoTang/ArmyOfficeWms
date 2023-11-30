using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.Common;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.HangChaAgv.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.HangChaAgv.ViewModel
{
    public class HangChaWorkerViewModel : OrderWorkerViewModel<HangChaWorkerBusiness>
    {
        public HangChaWorkerViewModel(DispatchWorkerAbstract<HangChaWorkerBusiness> worker)
            : base(worker)
        {
           
        }
       
    }
}
