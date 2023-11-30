using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.SwitchingWorker.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.SwitchingWorker.ViewModel
{
    public class SwitchingWorkerDeviceViewModel : WorkerViewModelAbstract<SwtichingWorkerAbstract<SwitchingWorkerBusinessAbstract>>
    {
        public SwitchingWorkerDeviceViewModel(SwtichingWorkerAbstract<SwitchingWorkerBusinessAbstract> worker)
            : base(worker)
        {
        }

        public override void NotifyAttributeChange(string attributeName, object newValue)
        {

        }
    }
}
