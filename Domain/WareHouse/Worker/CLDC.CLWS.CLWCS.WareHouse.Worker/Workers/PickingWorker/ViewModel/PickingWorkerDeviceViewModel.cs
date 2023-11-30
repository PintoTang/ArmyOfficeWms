using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.PickingWorker.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.PickingWorker.ViewModel
{
    public class PickingWorkerDeviceViewModel : WorkerViewModelAbstract<PickingWorkerAbstract<PickingWorkerBusinessAbstract>>
    {
        public PickingWorkerDeviceViewModel(PickingWorkerAbstract<PickingWorkerBusinessAbstract> worker)
            : base(worker)
        {
        }

        public override void NotifyAttributeChange(string attributeName, object newValue)
        {

        }
    }
}
