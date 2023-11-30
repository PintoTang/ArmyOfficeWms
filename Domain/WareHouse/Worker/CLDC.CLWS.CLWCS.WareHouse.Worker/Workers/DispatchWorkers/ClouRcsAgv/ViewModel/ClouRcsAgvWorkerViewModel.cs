using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.Common;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.ClouRcsAgv.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.ClouRcsAgv.ViewModel
{
    public class ClouRcsAgvWorkerViewModel : OrderWorkerViewModel<ClouRcsAgvWorkerBusiness>
    {
        public ClouRcsAgvWorkerViewModel(DispatchWorkerAbstract<ClouRcsAgvWorkerBusiness> worker)
            : base(worker)
        {
           
        }
       
    }
}
