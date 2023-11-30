using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.InAndOutCell.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.InAndOutCell.ViewModel
{
    public class InAndOutWorkerViewModel : OrderWorkerViewModel<InAndOutCellBusinessAbstract>
    {
        public InAndOutWorkerViewModel(InAndOutCellWorkerAbstract worker)
            : base(worker)
        {

        }

    }
}
