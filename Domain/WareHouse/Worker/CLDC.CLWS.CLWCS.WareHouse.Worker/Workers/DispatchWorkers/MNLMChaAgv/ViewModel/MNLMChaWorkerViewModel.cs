using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.Common;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.HangChaAgv.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.MNLMChaAgv.ViewModel
{
    public class MNLMChaWorkerViewModel : OrderWorkerViewModel<MNLMChaWorkerBusiness>
    {
        public MNLMChaWorkerViewModel(DispatchWorkerAbstract<MNLMChaWorkerBusiness> worker)
            : base(worker)
        {
           
        }
       
    }
}
