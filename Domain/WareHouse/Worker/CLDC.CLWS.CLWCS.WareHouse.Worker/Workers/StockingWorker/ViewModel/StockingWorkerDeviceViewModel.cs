using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.PickingWorker.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.StockingWorker.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.StockingWorker.ViewModel
{
    public class StockingWorkerDeviceViewModel : WorkerViewModelAbstract<StockingWorkerAbstract<StockingWorkerBusinessAbstract>>
    {
        public StockingWorkerDeviceViewModel(StockingWorkerAbstract<StockingWorkerBusinessAbstract> worker)
            : base(worker)
        {
        }

        public override void NotifyAttributeChange(string attributeName, object newValue)
        {

        }
    }
}
