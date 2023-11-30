using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.PalletierWorkers.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.PalletierWorkers.ViewModel
{
    public class PalletierWorkerDeviceViewModel : WorkerViewModelAbstract<PalletierWorkerAbstract<PalletierWorkerBusinessAbstract>>
    {
        public PalletierWorkerDeviceViewModel(PalletierWorkerAbstract<PalletierWorkerBusinessAbstract> worker)
            : base(worker)
        {
        }

     
        public override void NotifyAttributeChange(string attributeName, object newValue)
        {
          
        }
    }
}
